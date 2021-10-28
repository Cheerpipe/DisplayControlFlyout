using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using DisplayControlFlyout.ViewModels;
using DisplayControlFlyout.Views;

namespace DisplayControlFlyout.Services
{
    public class ApplicationTrayIcon
    {
        public readonly TrayIcon TrayIcon = new TrayIcon();

        public ApplicationTrayIcon()
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var icon = new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.dup_single.ico")));
            TrayIcon.Clicked += TrayIcon_Clicked;
            TrayIcon.Menu = new NativeMenu();
            NativeMenuItem exitMenu = new NativeMenuItem("Exit Display Control Flyout");
            exitMenu.Click += ExitMenu_Click;
            TrayIcon.Menu.Items.Add(exitMenu);
        }

        private void ExitMenu_Click(object? sender, EventArgs e)
        {
            Program.runCancellationTokenSource.Cancel();
        }

        private void TrayIcon_Clicked(object? sender, System.EventArgs e)
        {
            if (Program.MainWindowInstance == null)
            {
                Program.MainWindowInstance = new MainWindow();
                MainWindow flyout = Program.MainWindowInstance;
                flyout.DataContext = new DisplayControlViewModel();
                flyout.ShowAnimated();
            }
        }

        public void UpdateIcon()
        {
            SetIcon(DisplayManager.GetCurrentMode());
        }

        public void SetIcon(DisplayMode mode)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            switch (mode)
            {
                case DisplayMode.DuplicatedSingle:
                    Program.ApplicationTrayIconInstance.TrayIcon.Icon = new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.dup_single.ico")));
                    break;
                case DisplayMode.ExtendedAll:
                    Program.ApplicationTrayIconInstance.TrayIcon.Icon = new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.extended_all.ico")));
                    break;
                case DisplayMode.ExtendedHorizontal:
                    Program.ApplicationTrayIconInstance.TrayIcon.Icon = new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.extended_horizontal.ico")));
                    break;
                case DisplayMode.ExtendedHorizontalDuplicatedVertical:
                    Program.ApplicationTrayIconInstance.TrayIcon.Icon = new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.extended_plus_duplicated.ico")));
                    break;
                case DisplayMode.ExtendedSingle:
                    Program.ApplicationTrayIconInstance.TrayIcon.Icon = new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.ext_single.ico")));
                    break;
                case DisplayMode.Tv:
                    Program.ApplicationTrayIconInstance.TrayIcon.Icon = new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.tv.ico")));
                    break;
                case DisplayMode.Single:
                    Program.ApplicationTrayIconInstance.TrayIcon.Icon = new WindowIcon(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.Icons.single.ico")));
                    break;
                case DisplayMode.Unknown:
                    break;
            }
        }
    }
}