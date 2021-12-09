using System;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using DisplayControlFlyout.Extensions;
using DisplayControlFlyout.Services.FlyoutServices;
using AvaloniaTrayIcon = Avalonia.Controls.TrayIcon;

namespace DisplayControlFlyout.Services.TrayIcon
{
    public class TrayIconService : ITrayIconService, IDisposable
    {
        private readonly IFlyoutService _flyoutService;
        private readonly AvaloniaTrayIcon _trayIcon;
        private readonly Timer _alwaysVisibletimer;

        public TrayIconService(IFlyoutService flyoutService)
        {
            _flyoutService = flyoutService;
            _flyoutService.PreLoad();
            _trayIcon = new AvaloniaTrayIcon();
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var icon = new WindowIcon(DisplayMode.DuplicatedSingle.ToBitMap());
            _trayIcon.Icon = icon;
            _trayIcon.Clicked += TrayIcon_Clicked;
            _alwaysVisibletimer = new Timer(1000);
            _alwaysVisibletimer.Elapsed += _alwaysVisibletimer_Elapsed;
        }

        public void Refresh()
        {
            _trayIcon.IsVisible = false;
            _trayIcon.IsVisible = true;
        }

        private void _alwaysVisibletimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_trayIcon.IsVisible)
                _trayIcon.IsVisible = true;
        }

        public void Show()
        {
            _trayIcon.Menu = new NativeMenu();
            NativeMenuItem exitMenu = new("Exit Display Modes Flyout");
            exitMenu.Click += ExitMenu_Click;
            _trayIcon.Menu.Items.Add(exitMenu);
            _trayIcon.IsVisible = true;
        }
        public void Hide()
        {
            _trayIcon.IsVisible = false;
        }

        public void SetIcon(WindowIcon icon)
        {
            _trayIcon.Icon = icon;
        }

        private void ExitMenu_Click(object sender, EventArgs e)
        {
            Program.RunCancellationTokenSource.Cancel();
        }

        private void TrayIcon_Clicked(object sender, EventArgs e)
        {
            _flyoutService.Toggle();
        }

        public void Dispose()
        {
            _trayIcon?.Dispose();
        }
    }
}
