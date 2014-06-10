using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls;
using Wordbook.Converters;
using Wordbook.Services;
using Wordbook.Views;

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
        private static readonly string NoWord = "no words!";
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

            InteractionService.On(Interactions.Navigate, parameter =>
            {
                var options = parameter as NavigateOptions;
                if (options != null)
                {
                    if (options.Route == Routes.Words)
                    {
                        this.MainContentControl.Content = ViewLocator.WordsView;
                        this.StatusBarContentControl.Content = ViewLocator.WordsFilterView;
                    }
                    else if (options.Route == Routes.Edit)
                    {
                        var flyoutOptions = options.Parameter as FlyoutOptions;
                        if (flyoutOptions != null)
                        {
                            if (flyoutOptions.IsOpen)
                            {
                                if (!this.MainFlyout.IsOpen)
                                {
                                    var binding = new Binding("ActualWidth")
                                    {
                                        Source = this.Window,
                                        Converter = new SizeRatioConverter(),
                                        ConverterParameter = 0.64,
                                    };
                                    this.MainFlyout.SetBinding(FrameworkElement.WidthProperty, binding);


                                    this.MainFlyout.Header = "Edit";
                                    this.MainFlyout.Content = ViewLocator.EditView;
                                    this.MainFlyout.IsOpen = true;
                                }
                            }
                            else
                            {
                                this.MainFlyout.IsOpen = false;
                            }
                        }
                    }
                    else if (options.Route == Routes.Settings)
                    {
                        var flyoutOptions = options.Parameter as FlyoutOptions;
                        if (flyoutOptions != null)
                        {
                            if (flyoutOptions.IsOpen)
                            {
                                var binding = new Binding("ActualWidth")
                                {
                                    Source = this.Window,
                                    Converter = new SizeRatioConverter(),
                                    ConverterParameter = 0.64,
                                };
                                this.MainFlyout.SetBinding(FrameworkElement.WidthProperty, binding);

                                this.MainFlyout.Header = "Settings";
                                this.MainFlyout.Content = ViewLocator.SettingsView;
                                this.MainFlyout.IsOpen = true;
                            }
                            else
                            {
                                this.MainFlyout.IsOpen = false;
                            }
                        }
                    }
                }
            });
        }

        private void MainFlyoutOnIsOpenChanged(object sender, EventArgs e)
        {
            if (this.MainFlyout.IsOpen)
            {
                this.MainFlyout.Focus();
            }
            else
            {
                this.MainContentControl.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}
