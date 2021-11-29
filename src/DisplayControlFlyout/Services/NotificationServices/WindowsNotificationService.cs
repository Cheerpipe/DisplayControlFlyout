using System;
using CommunityToolkit.WinUI.Notifications;

namespace DisplayControlFlyout.Services
{
    public class WindowsNotificationService : INotificationServices
    {
        public void Show(string title, string content, Uri imageUri)
        {
            var x = new ToastContentBuilder()
                .AddText("Display mode changed")
                .AddAppLogoOverride(imageUri, ToastGenericAppLogoCrop.None, title)

                .AddText(content);
            x.Show();

        }
    }
}
