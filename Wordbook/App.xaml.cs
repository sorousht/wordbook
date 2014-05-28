using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro;

namespace Wordbook
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Semaphore _semaphore;
        bool _shouldRelease = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            var result = Semaphore.TryOpenExisting("Wordbook", out _semaphore);

            if (result) // we have another instance running
            {
                Application.Current.Shutdown();
            }
            else
            {
                try
                {
                    _semaphore = new Semaphore(1, 1, "Wordbook");
                }
                catch
                {
                    Application.Current.Shutdown(); //
                }
            }

            if (!_semaphore.WaitOne(0))
            {
                Application.Current.Shutdown();
            }
            else
            {
                _shouldRelease = true;
            }


            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_semaphore != null && _shouldRelease)
            {
                _semaphore.Release();
            }
        }

    }
}
