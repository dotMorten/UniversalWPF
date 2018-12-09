using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class ApplicationInitializer
    {
        internal static System.Threading.Thread UIThread;
        internal static System.Windows.Window window;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            var waitForApplicationRun = new TaskCompletionSource<bool>();
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            //Task.Run(() =>
            {
                var application = new System.Windows.Application();
            
                application.Startup += (s, e) => {
                    application.Dispatcher.Invoke(() =>
                    {
                      application.MainWindow = window = new System.Windows.Window();
                      window.Content = new System.Windows.Controls.ContentControl();
                      window.Show();
                      waitForApplicationRun.SetResult(true);
                    });
                };
                application.Run();
                return;
            }));
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            UIThread = t;
            waitForApplicationRun.Task.Wait();
        }


        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            window?.Dispatcher?.Invoke(() => { window?.Close(); });

            // UIThread.Abort();
            var app = System.Windows.Application.Current;
            if( app != null)
            {
                var d = app.Dispatcher;
                if (d != null && !(d.HasShutdownStarted || d.HasShutdownFinished))
                {
                    try
                    {
                        d.Invoke(() =>
                        {
                            if (!(d.HasShutdownStarted || d.HasShutdownFinished))
                                app.Shutdown();
                        });
                    }
                    catch (TaskCanceledException) { }
                }
            }
        }
    }
}
