using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DisplayControlFlyout.Services;

namespace DisplayControlFlyout.Extensions
{
    internal static class EnumExtensions
    {

        public static WindowIcon ToWindowIcon(this DisplayMode displayMode)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            return new WindowIcon(assets.Open(new Uri($@"resm:DisplayControlFlyout.Assets.Icons.{displayMode}.ico")));
        }

        public static Bitmap ToBitMap(this DisplayMode displayMode)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            return new Bitmap(assets.Open(new Uri($@"resm:DisplayControlFlyout.Assets.{displayMode}.png")));
        }

        public static Uri ToUriPackImage(this DisplayMode displayMode)
        {
            var imageResourcePath = System.IO.Path.Combine(AppContext.BaseDirectory, displayMode.ToString() + ".png");

            if (!System.IO.File.Exists(imageResourcePath))
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                using var bitmap = new Bitmap(assets.Open(new Uri($@"resm:DisplayControlFlyout.Assets.{displayMode}.png")));
                var imagePath = System.IO.Path.Combine(AppContext.BaseDirectory, displayMode.ToString() + ".png");
                bitmap.Save(imagePath);
            }

            return new Uri(imageResourcePath);
        }
    }
}
