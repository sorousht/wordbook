﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Threading;
using ReactiveUI;
using Wordbook.Data;
using Wordbook.Services;

namespace Wordbook.Controllers
{
    public class MainController : ReactiveObject
    {
        private readonly string DbFileName = "db.xml";

        public MainController()
        {
            this.InitializeCommand = new ReactiveCommand();
            this.InitializeCommand.RegisterAsyncAction(_ => this.Initialize());
        }

        private void Initialize()
        {
            this.IsInitializing = true;

            this.WordTypes = Enum.GetNames(typeof(WordType)).Select(type => (WordType)Enum.Parse(typeof(WordType), type));
            this.WordType = WordType.Noun;

            this.WhenAny(controller => controller.Keyword,
                (keyword) => this.SearchWords(keyword.Value))
                .Subscribe(words => this.Words = words);


            this.SaveCommand = new ReactiveCommand(this.WhenAny(
                controller => controller.WordText,
                controller => controller.WordType,
                (text, type) => !string.IsNullOrWhiteSpace(text.Value) && type.Value.GetHashCode() != 0));

            this.RemoveCommand = new ReactiveCommand(this.WhenAny(controller => controller.WordText, (text) => !string.IsNullOrWhiteSpace(text.Value)));

            this.CreateWordCommand = new ReactiveCommand(this.WhenAny(controller => controller.Keyword, (keyword) => !string.IsNullOrWhiteSpace(keyword.Value)));

            this.SaveCommand.RegisterAsyncAction(_ => this.Save(), Scheduler.CurrentThread);

            this.RemoveCommand.RegisterAsyncAction(_ => this.Remove(), Scheduler.CurrentThread);

            this.CreateWordCommand.RegisterAsyncAction(keyword => this.Create(keyword), Scheduler.CurrentThread);

            this.IsInitializing = false;
        }

        private ObservableCollection<Word> SearchWords(string keyword)
        {
            using (var context = new XmlContext(DbFileName))
            {
                return new ObservableCollection<Word>(context.GetWords(keyword));
            }
        }

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
            set { this.RaiseAndSetIfChanged(ref this._words, value); }
        }

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
            set
            {
                this._word = value;
                if (!this.IsInitializing)
                {
                    this.IsEditMode = true;
                    this.RaisePropertyChanged("WordText");
                    this.RaisePropertyChanged("WordDefinition");
                    this.RaisePropertyChanged("WordType");
                }
            }
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

        public bool IsInitializing { get; set; }

        private void Save()
        {
            using (var context = new XmlContext(DbFileName))
            {
                context.SaveWord(this.Word);
            }
            this.Words.Add(Word);
        }

        private void Remove()
        {
            using (var context = new XmlContext(DbFileName))
            {
                context.Remove(this.Word);
            }
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