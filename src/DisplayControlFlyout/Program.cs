using System;
using Avalonia;
using System.Threading;
using ArtemisFlyout.IoC;
using ArtemisFlyout.Services;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using DisplayControlFlyout.Services;
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
                    trayIconService.SetIcon(new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.dup_single.ico"))));
                    break;
                case DisplayMode.ExtendedAll:
                    trayIconService.SetIcon(new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.extended_all.ico"))));
                    break;
                case DisplayMode.ExtendedHorizontal:
                    trayIconService.SetIcon(new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.extended_horizontal.ico"))));
                    break;
                case DisplayMode.ExtendedHorizontalDuplicatedVertical:
                    trayIconService.SetIcon(new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.extended_plus_duplicated.ico"))));
                    break;
                case DisplayMode.ExtendedSingle:
                    trayIconService.SetIcon(new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.ext_single.ico"))));
                    break;
                case DisplayMode.Tv:
                    trayIconService.SetIcon(new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.tv.ico"))));
                    break;
                case DisplayMode.Single:
                    trayIconService.SetIcon(new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.single.ico"))));
                    break;
                case DisplayMode.Unknown:
                    break;
            }
        }
    }
}
