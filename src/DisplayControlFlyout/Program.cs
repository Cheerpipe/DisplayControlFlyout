using Avalonia;
using System.Threading;
using System.Windows.Forms;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using DisplayControlFlyout.Extensions;
using DisplayControlFlyout.IoC;
using DisplayControlFlyout.Services;
using DisplayControlFlyout.Services.FlyoutServices;
using DisplayControlFlyout.Services.IKeyboardHookServices;
using DisplayControlFlyout.Services.TrayIcon;
using DisplayControlFlyout.ViewModels;
using Microsoft.Win32;
using Application = Avalonia.Application;
using CommandLine;
using System;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;

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
        public static void Main(string[] args) => BuildAvaloniaApp().Start(AppMain, args);

        // Application entry point. Avalonia is completely initialized.
        static void AppMain(Application app, string[] args)
        {
            // Start IoC Kernel
            Kernel.Initialize(new Bindings());

            // Parse command line options
            bool exit = false;
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                  .WithParsed<CommandLineOptions>(o =>
                  {
                      if (Enum.TryParse(o.Mode, true, out DisplayMode sMode))
                      {
                          Task.Run(async () => {
                              await DisplayManager.SetMode(sMode, false).ConfigureAwait(false);
                          }).Wait();
                          exit = true;
                      }

                      if (o.Hdr != null)
                      {
                          HDR.SetGlobalHDRState((bool)o.Hdr);
                          exit = true;
                      }
                  });

            if (exit)
            {
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.Shutdown();
                }
                return;
            }

            // Instance control
            IInstanceService instanceService = Kernel.Get<IInstanceService>();
            if (instanceService.IsAlreadyRunning())
            {
                return;
            }

            // A cancellation token source that will be used to stop the main loop
            var cts = new CancellationTokenSource();

            IFlyoutService flyoutService = Kernel.Get<IFlyoutService>();
            flyoutService.SetPopulateViewModelFunc(() =>
            {
                return Kernel.Get<FlyoutContainerViewModel>();
            });

            Television.StartMonitor();
            var trayIconService = Kernel.Get<ITrayIconService>();
            trayIconService.Show();
            UpdateTrayIcon(trayIconService);

            SystemEvents.DisplaySettingsChanged += (_, _) =>
            {
                UpdateTrayIcon(trayIconService);
            };

            //Register Keyboard Shorcuts
            IKeyboardHookServices _keyboardHookServices = Kernel.Get<IKeyboardHookServices>();
            _keyboardHookServices.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Shift, Keys.D);
            _keyboardHookServices.KeyPressed += _keyboardHookServices_KeyPressed;


            // Start the main loop
            app.Run(RunCancellationToken);

            // Stop things
            trayIconService.Hide();
            Television.StopMonitor();
            Television.Dispose();
        }

        private static void _keyboardHookServices_KeyPressed(object? sender, KeyPressedEventArgs e)
        {
            if (e.Key == Keys.D && e.Modifier == (ModifierKeys.Win | ModifierKeys.Shift))
            {
                IFlyoutService flyoutService = Kernel.Get<IFlyoutService>();
                flyoutService.Show();
            }
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
