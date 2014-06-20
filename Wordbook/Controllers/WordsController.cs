using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using NAudio.Wave;
using ReactiveUI;
using WMPLib;
using Wordbook.Data;
using Wordbook.Properties;
using Wordbook.Services;

namespace Wordbook.Controllers
{
    public class WordsController : ReactiveObject
    {
        #region Constants

        private readonly Dictionary<int, DateTime> _timeRanges = new Dictionary<int, DateTime>
        {
            {0, DateTime.Now.Date},
            {1, DateTime.Now.AddDays(-1).Date},
            {2, DateTime.Now.AddDays(-7).Date},
            {3, DateTime.Now.AddMonths(-1).Date},
            {4, DateTime.Now.AddMonths(-6).Date},
            {5, DateTime.Now.AddYears(-1).Date},
        };

        private static readonly string PronounciationApiUri =
            "http://translate.google.com/translate_tts?ie=UTF-8&tl=en-us&q={0}";

        #endregion

        public WordsController()
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

                this.Context = new WordService(SettingService.Current.CurrentDatabase);

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

                this.PronounceCommand = new ReactiveCommand();

                this.SwitchEditModeCommand = new ReactiveCommand();
                this.SwitchEditModeCommand.Subscribe(SwitchEditMode);


                this.SaveCommand.Subscribe(_ => this.Save());

                this.RemoveCommand.Subscribe(_ => this.Remove());

                this.CreateWordCommand.Subscribe(this.Create);

                this.PronounceCommand.Subscribe(Pronounce);

                this.WhenAny(controller => controller.Keyword, controller => controller.TimePeriod,
                    (keyword, period) =>
                    {
                        this.ChosenTimePeriod = period.Value;
                        return this.SearchWords(keyword.Value, this._timeRanges[period.Value]);
                    })
                    .Subscribe(words =>
                    {
                        this.Words = words;
                        if (this.Words.Count > 0)
                        {
                            this.Word = this.Words[0];
                        }
                    });


                this.WhenAny(controller => controller.Word, (word) => word).Subscribe(word =>
                {
                    if (!this.IsInitializing && word.Value != null)
                    {
                        InteractionService.OpenFlyout(Routes.Edit);

                        this.RaisePropertyChanged("WordText");
                        this.RaisePropertyChanged("WordDefinition");
                        this.RaisePropertyChanged("WordType");

                        this.ChosenWordRegisteredSetting = word.Value.Registered.HasValue
                            ? word.Value.Registered.Value
                            : DateTime.Today;

                    }
                });

                if (this.Words != null && this.Words.Any())
                {
                    this.Word = this.Words.SingleOrDefault(word => word.Registered == this.ChosenWordRegisteredSetting) ??
                                this.Words.FirstOrDefault();
                }

                this.SoundPlayer = new WaveOut();

                var pronounciationFilePath = Path.Combine(Environment.CurrentDirectory,
                    ConfigurationManager.AppSettings["PronounciationDatabaseName"]);

                this.PronounciationService = new PronounciationService(pronounciationFilePath);

                this.IsInitializing = false;
            });

            EventAggrigator.Subscribe("CurrentDatabaseChanged", parameter =>
            {
                if (parameter != null)
                {
                    var currentDatabase = parameter.ToString();
                    if (!string.IsNullOrWhiteSpace(currentDatabase))
                    {
                        this.Context = new WordService(currentDatabase);
                        this.Words = this.SearchWords(this.Keyword, this._timeRanges[this.TimePeriod]);
                    }
                }
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

        private ReactiveCommand _pronounceCommand;
        public ReactiveCommand PronounceCommand
        {
            get { return this._pronounceCommand; }
            set { this.RaiseAndSetIfChanged(ref this._pronounceCommand, value); }
        }

        #endregion

        private WaveOut SoundPlayer { get; set; }

        private WordService Context { get; set; }

        private PronounciationService PronounciationService { get; set; }

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
            var words = new ObservableCollection<Word>(Context.Search(keyword, range));

            InteractionService.Notify(new NotifyOptions(States.WordsLoaded, words.Count));

            return words;
        }

        private void Save()
        {
            if (this.Context.Exists(this.Word))
            {
                this.ResetWord(this.Word);

                this.Context.Save();

                InteractionService.Notify(new NotifyOptions(States.WordUpdated, this.WordText));
            }
            else
            {
                this.Word.Registered = DateTime.Now;

                this.Context.Items.Add(this.Word);

                this.Words.Insert(0, this.Word);

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

            this.Context.Items.Remove(this.Word);

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

            InteractionService.OpenFlyout(Routes.Edit);
        }

        private void SwitchEditMode(object parameter)
        {
            var isOpen = Convert.ToBoolean(parameter);
            if (isOpen)
            {
                InteractionService.OpenFlyout(Routes.Edit);
            }
            else
            {
                InteractionService.CloseFlyout();
            }
        }

        private async void Pronounce(object parameter)
        {
            byte[] buffer = null;

            if (parameter != null)
            {
                var word = parameter as Word;

                if (word != null && word.Registered.HasValue)
                {
                    if (this.PronounciationService.Exists(word.Registered.Value))
                    {
                        buffer = this.PronounciationService.Get(word.Registered.Value).Sound;
                    }
                    else
                    {
                        using (var client = new WebClient())
                        {
                            try
                            {
                                buffer = await client.DownloadDataTaskAsync(string.Format(WordsController.PronounciationApiUri, word.Text));
                            }
                            catch (WebException exception)
                            {
                                InteractionService.Notify(new NotifyOptions(States.UnableToConnect, null));
                            }
                        }

                        this.PronounciationService.Items.Add(new Pronounciation(word.Registered.Value, buffer));
                        this.PronounciationService.Save();
                    }

                    if (buffer != null && buffer.Length > 0)
                    {
                        this.SoundPlayer.Stop();
                        this.SoundPlayer.Init(new Mp3FileReader(new MemoryStream(buffer)));
                        this.SoundPlayer.Play();
                    }
                }
            }
        }
    }
}