using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Legacy;
using Wordbook.Data;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace Wordbook.Controllers
{
    public class EditController : ReactiveObject
    {
        public EditController()
        {
            this.InitializeCommand = new ReactiveCommand();
            this.InitializeCommand.Subscribe(_ => this.Initialize());
        }

        private void Initialize()
        {
            this.SaveCommand = new ReactiveCommand();
            this.SaveCommand.Subscribe(_ => this.Save());

            this.WordTypes = Enum.GetNames(typeof (WordType)).Select(type => (WordType) Enum.Parse(typeof (WordType), type));
            this.SelectedWordType = WordType.Noun;
        }

        public ReactiveCommand InitializeCommand { get; set; }
        public ReactiveCommand SaveCommand { get; set; }

        private Word _word;
        public Word Word { get { return this._word; } set { this.RaiseAndSetIfChanged(ref this._word, value); } }

        private IEnumerable<WordType> _wordTypes;
        public IEnumerable<WordType> WordTypes { get { return this._wordTypes; } set { this.RaiseAndSetIfChanged(ref this._wordTypes, value); } }

        private WordType _selectedWordType;
        public WordType SelectedWordType { get { return this._selectedWordType; } set { this.RaiseAndSetIfChanged(ref this._selectedWordType, value); } }


        private void Save()
        {
            using (var context = new XmlContext("db.xml"))
            {
                context.SaveWord(this.Word);
            }
        }
    }
}