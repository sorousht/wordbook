using System;
using System.Collections.Generic;
using ReactiveUI;
using Wordbook.Data;
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
            this.Databases = SettingService.Current.Databases;
            this.SelectedDatabase = SettingService.Current.CurrentDatabase;

            this.AddDatabaseCommand = new ReactiveCommand();
            this.AddDatabaseCommand.Subscribe(AddDatabase);

            this.WhenAny(controller => controller.SelectedDatabase, (currentDb) =>
            {
                return currentDb.Value;

            }).Subscribe(value =>
            {
                if (!string.IsNullOrWhiteSpace(value) && SettingService.Current.CurrentDatabase != value)
                {
                    SettingService.Current.CurrentDatabase = value;
                    SettingService.Save();

                    EventAggrigator.Publish("CurrentDatabaseChanged", value);
                }
            });

            this.WhenAny(controller => controller.Databases, (databases) =>
            {
                return databases.Value;

            }).Subscribe(value =>
            {
                if (value != null)
                {
                    SettingService.Current.Databases = value;
                    SettingService.Save();
                }
            });
        }

        public ReactiveCommand InitializeCommand { get; set; }

        private IList<string> _databases;
        public IList<string> Databases
        {
            get { return this._databases; }
            set { this.RaiseAndSetIfChanged(ref this._databases, value); }
        }

        private string _selectedDatabase;
        public string SelectedDatabase
        {
            get { return this._selectedDatabase; }
            set { this.RaiseAndSetIfChanged(ref this._selectedDatabase, value); }
        }

        private ReactiveCommand _addDatabaseCommand;
        public ReactiveCommand AddDatabaseCommand
        {
            get { return this._addDatabaseCommand; }
            set { this.RaiseAndSetIfChanged(ref this._addDatabaseCommand, value); }
        }
        private void AddDatabase(object parameter)
        {

        }

    }
}