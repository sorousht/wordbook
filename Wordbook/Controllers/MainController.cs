using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Wordbook.Data;
using Wordbook.Properties;
using Wordbook.Services;

namespace Wordbook.Controllers
{
    public class MainController : ReactiveObject
    {
        #region Constants

        private readonly string DbFileName = "db.xml";

        private readonly Dictionary<int, DateTime> _timeRanges = new Dictionary<int, DateTime>
        {
            {0, DateTime.Now.Date},
            {1, DateTime.Now.AddDays(-1).Date},
            {2, DateTime.Now.AddDays(-7).Date},
            {3, DateTime.Now.AddMonths(-1).Date},
            {4, DateTime.Now.AddMonths(-6).Date},
            {5, DateTime.Now.AddYears(-1).Date},
        };

        #endregion

        public MainController()
        {
            this.Word = new Word();
            this.InitializeCommand = new ReactiveCommand();
            this.InitializeCommand.Subscribe(_ => this.Initialize());
        }

        private async void Initialize()
        {
            await Task.Run(() =>
            {
                this.IsInitializing = true;

                this.Context = new XmlContext(DbFileName);

                this.TimePeriod = this.ChosenTimePeriod;

                this.WordTypes =
                    Enum.GetNames(typeof(WordType)).Select(type => (WordType)Enum.Parse(typeof(WordType), type));
                this.WordType = WordType.Noun;

                this.SaveCommand = new ReactiveCommand(this.WhenAny(
                    controller => controller.WordText,
                    controller => controller.WordType,
                    (text, type) => !string.IsNullOrWhiteSpace(text.Value) && type.Value.GetHashCode() != 0));

                this.RemoveCommand =
                    new ReactiveCommand(this.WhenAny(controller => controller.Word,
                        (word) => word.Value != null && word.Value.Registered.HasValue));

                this.CreateWordCommand = new ReactiveCommand();

                this.SwitchEditModeCommand = new ReactiveCommand();
                this.SwitchEditModeCommand.Subscribe(SwitchEditMode);


                this.SaveCommand.Subscribe(_ => this.Save());

                this.RemoveCommand.Subscribe(_ => this.Remove());

                this.CreateWordCommand.Subscribe(this.Create);

                this.WhenAny(controller => controller.Keyword, controller => controller.TimePeriod,
                    (keyword, period) =>
                    {
                        this.ChosenTimePeriod = period.Value;
                        return this.SearchWords(keyword.Value, this._timeRanges[period.Value]);
                    })
                    .Subscribe(words =>
                    {
                        this.Words = words;
                    });


                this.WhenAny(controller => controller.Word, (word) => word).Subscribe(word =>
                {
                    if (!this.IsInitializing && word.Value != null)
                    {
                        this.IsEditMode = true;

                        this.RaisePropertyChanged("WordText");
                        this.RaisePropertyChanged("WordDefinition");
                        this.RaisePropertyChanged("WordType");

                        this.ChosenWordRegisteredSetting = word.Value.Registered.HasValue
                            ? word.Value.Registered.Value
                            : DateTime.Today;
                    }
                });

                if (this.Words != null)
                {
                    this.Word = this.Words.SingleOrDefault(word => word.Registered == this.ChosenWordRegisteredSetting) ??
                                this.Words.FirstOrDefault();
                }

                this.IsInitializing = false;
            });
        }

        #region Settings

        private DateTime _chosenWordRegisteredSetting;

        private DateTime ChosenWordRegisteredSetting
        {
            get { return DateTime.FromFileTimeUtc(Settings.Default.ChosenWordRegisteredUtc).ToLocalTime(); }
            set
            {
                if (_chosenWordRegisteredSetting != value)
                {
                    this._chosenWordRegisteredSetting = value;
                    Settings.Default.ChosenWordRegisteredUtc = value.ToFileTime();
                    Settings.Default.Save();
                }
            }
        }

        private int _chosenTimePeriod;

        private int ChosenTimePeriod
        {
            get { return Settings.Default.ChosenTimePeriod; }
            set
            {
                if (_chosenTimePeriod != value)
                {
                    this._chosenTimePeriod = value;
                    Settings.Default.ChosenTimePeriod = value;
                    Settings.Default.Save();
                }
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand InitializeCommand { get; set; }
        private ReactiveCommand _saveCommand;

        public ReactiveCommand SaveCommand
        {
            get { return this._saveCommand; }
            set { this.RaiseAndSetIfChanged(ref this._saveCommand, value); }
        }

        private ReactiveCommand _removeCommand;

        public ReactiveCommand RemoveCommand
        {
            get { return this._removeCommand; }
            set { this.RaiseAndSetIfChanged(ref this._removeCommand, value); }
        }

        private ReactiveCommand _createWord;

        public ReactiveCommand CreateWordCommand
        {
            get { return this._createWord; }
            set { this.RaiseAndSetIfChanged(ref this._createWord, value); }
        }

        private ReactiveCommand _switchEditModeCommand;

        public ReactiveCommand SwitchEditModeCommand
        {
            get { return this._switchEditModeCommand; }
            set { this.RaiseAndSetIfChanged(ref this._switchEditModeCommand, value); }
        }
        #endregion

        private XmlContext Context { get; set; }

        private ObservableCollection<Word> _words;
        public ObservableCollection<Word> Words
        {
            get { return this._words; }
            set { this.RaiseAndSetIfChanged(ref this._words, value); }
        }

        private Word _word;
        public Word Word
        {
            get { return this._word; }
            set { this.RaiseAndSetIfChanged(ref this._word, value); }
        }

        public string WordText
        {
            get
            {
                return this.Word != null ? this.Word.Text : null;
            }
            set
            {
                this.Word.Text = value;
                this.RaisePropertyChanged();
            }
        }
        public string WordDefinition
        {
            get
            {
                return this.Word != null ? this.Word.Definition : null;
            }
            set
            {
                this.Word.Definition = value;
                this.RaisePropertyChanged();
            }
        }

        private IEnumerable<WordType> _wordTypes;
        public IEnumerable<WordType> WordTypes
        {
            get { return this._wordTypes; }
            set { this.RaiseAndSetIfChanged(ref this._wordTypes, value); }
        }

        public WordType WordType
        {
            get { return this.Word != null ? this.Word.Type : WordType.Noun; }
            set
            {
                this.Word.Type = value;
                this.RaisePropertyChanged();
            }
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return this._isEditMode; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._isEditMode, value);
            }
        }

        private string _keyword;

        public string Keyword
        {
            get { return this._keyword; }
            set { this.RaiseAndSetIfChanged(ref this._keyword, value); }
        }

        private int _timePeriod;
        public int TimePeriod
        {
            get { return this._timePeriod; }
            set { this.RaiseAndSetIfChanged(ref this._timePeriod, value); }
        }

        public bool IsInitializing { get; set; }

        private ObservableCollection<Word> SearchWords(string keyword, DateTime range)
        {
            var words = new ObservableCollection<Word>(Context.GetWords(keyword, range));

            InteractionService.Notify(new NotifyOptions(States.WordsLoaded, words.Count));

            return words;
        }

        private void Save()
        {
            if (this.Context.Exists(this.Word))
            {
                var word = this.Word;

                this.Context.UpdateWord(word);

                this.ResetWord(word);

                InteractionService.Notify(new NotifyOptions(States.WordUpdated, this.WordText));
            }
            else
            {
                this.Context.AddWord(this.Word);
                this.Words.Add(this.Word);

                InteractionService.Notify(new NotifyOptions(States.WordAdded, this.WordText));
            }
        }

        private void ResetWord(Word word)
        {
            var index = this.Words.IndexOf(word);

            this.Words.RemoveAt(index);
            this.Words.Insert(index, word);

            this.Word = word;
        }

        private void Remove()
        {
            var index = this.Words.IndexOf(this.Word);

            var text = this.WordText;
            this.Context.Remove(this.Word);

            this.Words.Remove(this.Word);

            this.SelectNextWord(index);

            InteractionService.Notify(new NotifyOptions(States.WordRemoved, text));
        }

        private void SelectNextWord(int index)
        {
            if (this.Words.Count > index)
            {
                this.Word = this.Words[index];
            }
            else
            {
                this.Word = this.Words.LastOrDefault();
            }
        }

        private void Create(object keyword)
        {
            this.Word = new Word
            {
                Text = keyword.ToString(),
                Type = WordType.Noun
            };

            this.IsEditMode = true;
        }

        private void SwitchEditMode(object parameter)
        {
            this.IsEditMode = Convert.ToBoolean(parameter);
        }
    }
}