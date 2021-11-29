using Avalonia;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using DisplayControlFlyout.Extensions;
using DisplayControlFlyout.IoC;
using DisplayControlFlyout.Services;
using DisplayControlFlyout.Services.TrayIcon;
using Microsoft.Win32;

namespace DisplayControlFlyout
{
    public class Program
    {
        public static CancellationTokenSource RunCancellationTokenSource { get; } = new();

        private static readonly CancellationToken RunCancellationToken = RunCancellationTokenSource.Token;

        // This method is needed for IDE previewer infrastructure
        public static AppBuilder BuildAvaloniaApp()
        {
            var builder = AppBuilder.Configure<App>().UsePlatformDetect().UseReactiveUI().UseSkia().With(new Win32PlatformOptions()
            {
                UseWindowsUIComposition = true,
                CompositionBackdropCornerRadius = 10f,
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
            Kernel.Initialize(new Bindings());

            // Do you startup code here
            Television.StartMonitor();
            var trayIconService = Kernel.Get<ITrayIconService>();
            trayIconService.Show();
            UpdateTrayIcon(trayIconService);

            SystemEvents.DisplaySettingsChanged += (_, _) =>
            {
                UpdateTrayIcon(trayIconService);
            };

            // Start the main loop
            app.Run(RunCancellationToken);

            // Stop things
            trayIconService.Hide();
            Television.StopMonitor();
            Television.Dispose();
        }

        private static void UpdateTrayIcon(ITrayIconService trayIconService)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            switch (DisplayManager.GetCurrentMode())
            {
                case DisplayMode.DuplicatedSingle:
                    trayIconService.SetIcon(DisplayMode.DuplicatedSingle.ToWindowIcon());
                    break;
                case DisplayMode.ExtendedAll:
                    trayIconService.SetIcon(DisplayMode.ExtendedAll.ToWindowIcon());
                    break;
                case DisplayMode.ExtendedHorizontal:
                    trayIconService.SetIcon(DisplayMode.ExtendedHorizontal.ToWindowIcon());
                    break;
                case DisplayMode.ExtendedDuplicated:
                    trayIconService.SetIcon(DisplayMode.ExtendedDuplicated.ToWindowIcon());
                    break;
                case DisplayMode.ExtendedSingle:
                    trayIconService.SetIcon(DisplayMode.ExtendedSingle.ToWindowIcon());
                    break;
                case DisplayMode.Tv:
                    trayIconService.SetIcon(DisplayMode.Tv.ToWindowIcon());
                    break;
                case DisplayMode.Single:
                    trayIconService.SetIcon(DisplayMode.Single.ToWindowIcon());
                    break;
                case DisplayMode.Unknown:
                    break;
            }
        }
    }
}
