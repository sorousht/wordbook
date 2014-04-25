using System;
using ReactiveUI;
using Wordbook.Services;

namespace Wordbook.Controllers
{
    public class MainController : ReactiveObject
    {
        public MainController()
        {
            this.NavigateCommand = new ReactiveCommand();
            this.NavigateCommand.RegisterAsyncAction(page =>
            {
                if (page != null)
                {
                    this.Page = (AppPage)Enum.Parse(typeof(AppPage), page.ToString());
                }
            });
        }
        public AppPage Page
        {
            get
            {
                return NavigationService.Current;
            }
            set
            {
                NavigationService.Navigate(value);
                this.RaisePropertyChanged();
            }
        }

        public ReactiveCommand NavigateCommand { get; set; }
    }
}