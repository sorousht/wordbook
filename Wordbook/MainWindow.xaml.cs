using System;
using MahApps.Metro.Controls;
using Wordbook.Properties;

namespace Wordbook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
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
