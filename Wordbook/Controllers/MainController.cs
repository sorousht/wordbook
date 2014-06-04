using System;
using ReactiveUI;
using Wordbook.Services;

namespace Wordbook.Controllers
{
    public class MainController : ReactiveObject
    {
        public MainController()
        {
            this.InitializeCommand = new ReactiveCommand();
            this.InitializeCommand.Subscribe(_ => this.Initialize());
        }

        private void Initialize()
        {
            InteractionService.Navigate(Routes.Words);

            this.NavigateSettingsCommand = new ReactiveCommand();
            this.NavigateSettingsCommand.Subscribe(_ => InteractionService.OpenFlyout(Routes.Settings));
        }

        public ReactiveCommand InitializeCommand { get; set; }

        private ReactiveCommand _navigateSettingsCommand;
        public ReactiveCommand NavigateSettingsCommand
        {
            get { return this._navigateSettingsCommand; }
            set { this.RaiseAndSetIfChanged(ref this._navigateSettingsCommand, value); }
        }
    }
}