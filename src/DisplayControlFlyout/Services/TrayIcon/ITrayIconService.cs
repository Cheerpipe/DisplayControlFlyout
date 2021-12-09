using Avalonia.Controls;

namespace DisplayControlFlyout.Services.TrayIcon
{
    public interface ITrayIconService
    {
        void Show();
        void Hide();
        //TODO Move out of here
        void SetIcon(WindowIcon icon);
        void Refresh();
    }
}
