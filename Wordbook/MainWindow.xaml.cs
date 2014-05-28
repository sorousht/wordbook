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
        private static readonly string WordAddedMessage = "\"{0}\" has been added.";
        private static readonly string WordRemovedMessage = "\"{0}\" was removed.";
        private static readonly string WordUpdatedMessage = "\"{0}\" has been updated.";
        private static readonly string WordsLoaded = "{0} words was found.";
        private static readonly string AWordFound = "only one word was found.";
        private static readonly string NoWord = "there isn't any word!";

        public MainWindow()
        {
            InitializeComponent();

            InteractionService.On(Interactions.Notify, parameter => Dispatcher.Invoke(() =>
            {
                var options = parameter as NotifyOptions;
                if (options != null)
                {
                    switch (options.State)
                    {
                        case States.WordAdded:
                            this.StatusTextBlock.Text = string.Format(WordAddedMessage, options.Parameter);
                            break;
                        case States.WordRemoved:
                            this.StatusTextBlock.Text = string.Format(WordRemovedMessage, options.Parameter);
                            break;
                        case States.WordUpdated:
                            this.StatusTextBlock.Text = string.Format(WordUpdatedMessage, options.Parameter);
                            break;
                        case States.WordsLoaded:
                            var count = Convert.ToInt32(options.Parameter);
                            if (count == 0)
                            {
                                this.StatusTextBlock.Text = NoWord;
                            }
                            else if (count == 1)
                            {
                                this.StatusTextBlock.Text = AWordFound;
                            }
                            else
                            {
                                this.StatusTextBlock.Text = string.Format(WordsLoaded, count);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

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
