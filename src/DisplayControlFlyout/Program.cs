using Avalonia;
using System.Threading;
using Avalonia.Controls;
using DisplayControlFlyout.Views;
using Avalonia.ReactiveUI;
using DisplayControlFlyout.Services;
using Microsoft.Win32;

namespace DisplayControlFlyout
{
    public class Program
    {
        public static CancellationTokenSource runCancellationTokenSource = new CancellationTokenSource();
        public static MainWindow MainWindowInstance;
        public static ApplicationTrayIcon ApplicationTrayIconInstance;

        static CancellationToken runCancellationToken = runCancellationTokenSource.Token;

        // This method is needed for IDE previewer infrastructure
        public static AppBuilder BuildAvaloniaApp()
        {
            var builder = AppBuilder.Configure<App>().UsePlatformDetect().UseReactiveUI().UseSkia().With(new Win32PlatformOptions()
            {
                UseWindowsUIComposition = true
            }); ;
            return builder;
        }


        // The entry point. Things aren't ready yet, so at this point
        // you shouldn't use any Avalonia types or anything that expects
        // a SynchronizationContext to be ready
        public static void Main(string[] args)
            => BuildAvaloniaApp().Start(AppMain, args);

        // Application entry point. Avalonia is completely initialized.
        static void AppMain(Application app, string[] args)
        {
            // A cancellation token source that will be used to stop the main loop
            var cts = new CancellationTokenSource();

            // Do you startup code here

            Television.StartMonitor();

            ApplicationTrayIcon applicationTrayIconInstance = new ApplicationTrayIcon();
            ApplicationTrayIconInstance = applicationTrayIconInstance;
            ApplicationTrayIconInstance.UpdateIcon();

            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            // Start the main loop
            app.Run(runCancellationToken);
            Television.StopMonitor();
            Television.Dispose();
        }

        private static void SystemEvents_DisplaySettingsChanged(object sender, System.EventArgs e)
        {
            ApplicationTrayIconInstance.UpdateIcon();
        }
    }
}
