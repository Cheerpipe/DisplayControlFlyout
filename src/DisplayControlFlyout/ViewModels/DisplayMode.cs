
using Avalonia.Media.Imaging;
using DisplayControlFlyout.Services;

namespace DisplayControlFlyout.ViewModels
{
    public class ApplicableDisplayMode
    {
        public string DisplayName { get; set; }
        public DisplayMode Mode { get; set; }
        public Bitmap Image { get; set; }
    }
}
