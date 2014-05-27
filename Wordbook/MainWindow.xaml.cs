using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls;
using Wordbook.Properties;
using Wordbook.Services;

namespace Wordbook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static readonly IDictionary<States, string> Messages = new Dictionary<States, string>
        {
            {States.WordAdded, "{0} added."},
            {States.WordRemoved,"{0} removed."},
            {States.WordUpdated,"{0} updated."},
            {States.WordsLoaded,"{0} words found."},
        };

        public MainWindow()
        {
            InitializeComponent();

            InteractionService.On(Interactions.Notify, parameter => Dispatcher.Invoke(() =>
            {
                var options = parameter as NotifyOptions;
                if (options != null)
                {
                    this.StatusTextBlock.Text = string.Format(Messages[options.State], options.Parameter);
                    var storyboard = ((Storyboard)this.StatusTextBlock.Resources["TextChangeStoryboard"]);
                    storyboard.SkipToFill();
                    storyboard.Begin();
                }
            }));
        }

        private void EditFlyout_OnIsOpenChanged(object sender, EventArgs e)
        {
            if (this.EditFlyout.IsOpen)
            {
                this.EditFlyout.Focus();
            }
            else
            {
                this.WordsListBox.Focus();
            }
        }
    }
}
