using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Windows;
using ReactiveUI;
using Wordbook.Data;
using Wordbook.Properties;

namespace Wordbook
{
    public class MainController : ReactiveObject
    {
        private readonly string DbFileName = "db.xml";
        private readonly Dictionary<int, DateTime> _timeRanges = new Dictionary<int, DateTime>
        {
            {0,DateTime.Now.AddDays(-1).Date},
            {1,DateTime.Now.AddDays(-2).Date},
            {2,DateTime.Now.AddDays(-7).Date},
            {3,DateTime.Now.AddMonths(-1).Date},
            {4,DateTime.Now.AddMonths(-6).Date},
            {5,DateTime.Now.AddYears(-1).Date},
        };

        public MainController()
        {
            this.InitializeCommand = new ReactiveCommand();
            this.InitializeCommand.RegisterAsyncAction(_ => this.Initialize());
        }

        private void Initialize()
        {
            this.IsInitializing = true;

            //Last Week
            this.TimePeriod = Settings.Default.ChosenTimePeriod;

            this.Context = new XmlContext(DbFileName);

            this.WordTypes = Enum.GetNames(typeof(WordType)).Select(type => (WordType)Enum.Parse(typeof(WordType), type));
            this.WordType = WordType.Noun;

            this.SaveCommand = new ReactiveCommand(this.WhenAny(
                 controller => controller.WordText,
                 controller => controller.WordType,
                 (text, type) => !string.IsNullOrWhiteSpace(text.Value) && type.Value.GetHashCode() != 0));

            this.RemoveCommand = new ReactiveCommand(this.WhenAny(controller => controller.WordText, (text) => !string.IsNullOrWhiteSpace(text.Value)));

            this.CreateWordCommand = new ReactiveCommand();

            this.SaveCommand.RegisterAsyncAction(_ => this.Save(), Scheduler.CurrentThread);

            this.RemoveCommand.RegisterAsyncAction(_ => this.Remove(), Scheduler.CurrentThread);

            this.CreateWordCommand.RegisterAsyncAction(keyword => this.Create(keyword), Scheduler.CurrentThread);

            this.WhenAny(controller => controller.Keyword, controller => controller.TimePeriod,
               (keyword, period) =>
                   this.SearchWords(keyword.Value, this._timeRanges[period.Value]))
               .Subscribe(words =>
               {
                   this.Words = words;

                   this.Word = this.GetPreviouslyChosenWord();
               });

            this.WhenAny(controller => controller.Word, (word) => word).Subscribe(change =>
            {
                if (!this.IsInitializing)
                {
                    this.IsEditMode = true;
                    this.RaisePropertyChanged("WordText");
                    this.RaisePropertyChanged("WordDefinition");
                    this.RaisePropertyChanged("WordType");
                }
            });

            this.WhenAny(controller => controller.Words, (words) => words).Subscribe(change =>
            {
                this.IsEditMode = false;
            });

            this.WhenAny(controller => controller.TimePeriod, controller => controller.Word, (period, word) =>
            {
                Settings.Default.ChosenTimePeriod = period.Value;
                Settings.Default.ChosenWordRegisteredUtc = word.Value.Registered.ToFileTimeUtc();

                return true;
            }).Subscribe(_ => Settings.Default.Save());

            this.IsInitializing = false;
        }

        private Word GetPreviouslyChosenWord()
        {
            if (this.Word != null)
            {
                var chosenDateTime = DateTime.FromFileTimeUtc(Settings.Default.ChosenWordRegisteredUtc).ToLocalTime();
                return this.Words.SingleOrDefault(word => word.Registered == chosenDateTime) ??
                       this.Words.FirstOrDefault();
            }

            return null;

        }

        private XmlContext Context { get; set; }

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

        private ObservableCollection<Word> _words;
        public ObservableCollection<Word> Words
        {
            get { return this._words; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._words, value);
                if (this.Words != null)
                {
                    this.Words.CollectionChanged += (sender, args) =>
                    {
                        this.RaisePropertyChanged("WordsCount");
                    };
                }
                this.RaisePropertyChanged("WordsCount");
            }
        }

        public string WordsCount { get { return string.Format("{0} items.", this.Words != null ? this.Words.Count : 0); } }

        private Word _word;
        public Word Word
        {
            get
            {
                if (this._word == null)
                {
                    this._word = new Word
                    {
                        Text = string.Empty,
                        Type = WordType.Noun,
                    };
                }
                return this._word;
            }
            set { this.RaiseAndSetIfChanged(ref this._word, value); }
        }

        public string WordText
        {
            get
            {
                return this.Word.Text;
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
                return this.Word.Definition;
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
            get
            {
                return this.Word.Type;
            }
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
            return new ObservableCollection<Word>(Context.GetWords(keyword, range));
        }

        private void Save()
        {
            if (this.Context.Exists(this.Word))
            {
                var word = this.Word;

                this.Context.UpdateWord(word);

                this.ResetWord(word);
            }
            else
            {
                this.Context.AddWord(this.Word);
                this.Words.Add(this.Word);
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
            this.Context.Remove(this.Word);

            this.Words.Remove(this.Word);
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
    }
}