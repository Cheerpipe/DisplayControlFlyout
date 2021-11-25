using Avalonia.Controls;
using DisplayControlFlyout.Services;

namespace ArtemisFlyout.Services
{
    public interface ITrayIconService
    {
        void Show();
        void Hide();
        //TODO Move out of here
        void SetIcon(WindowIcon icon);
    }
}
