using System;
using ReactiveUI;
using Wordbook.Services;

namespace Wordbook.Controllers
{
    public class SettingsController : ReactiveObject
    {
        public SettingsController()
        {
            this.InitializeCommand = new ReactiveCommand();
            this.InitializeCommand.Subscribe(_ => this.Initialize());
        }

        private void Initialize()
        {

        }

        public ReactiveCommand InitializeCommand { get; set; }
    }
}