#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DisplayControlFlyout.Services;
using DisplayControlFlyout.ViewModels;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;
using Window = Avalonia.Controls.Window;

namespace DisplayControlFlyout.Views
{
    public partial class MainWindow : Window
    {
        private const int FlyoutHorizontalSpacing = 12;
        private const int FlyoutVerticalSpacing = 25;
        private const int AnimationDelay = 200;
        private const int FlyoutWidth = 280 + FlyoutHorizontalSpacing;
        private const int FlyoutHeight = 455 + FlyoutVerticalSpacing;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var primaryScreen = Screens.Primary.WorkingArea;

            WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.Manual;
            //TODO: Use taskbar height and binded Width and Height
            Position = new PixelPoint(primaryScreen.Width - FlyoutWidth, primaryScreen.Height - FlyoutHeight);
            Deactivated += MainWindow_Deactivated;
        }

        private bool _animating;
        public async void ShowAnimated()
        {
            if (_animating)
                return;
            _animating = true;

            Show();
            var filler = this.Find<Separator>("SepAnimationFiller");
            Width = FlyoutWidth;
            Height = FlyoutHeight;

            var t = new DoubleTransition()
            {

                Property = Separator.WidthProperty,
                Duration = TimeSpan.FromMilliseconds(AnimationDelay),
                Easing = new CircularEaseOut()
            };

            t.Apply(filler, Avalonia.Animation.Clock.GlobalClock, (double)FlyoutWidth, 0d);

            _animating = false;
        }

        public async void CloseAnimated()
        {
            if (_animating)
                return;
            _animating = true;

            var filler = this.Find<Separator>("SepAnimationFiller");
            var t = new DoubleTransition()
            {

                Property = Separator.WidthProperty,
                Duration = TimeSpan.FromMilliseconds(AnimationDelay),
                Easing = new CircularEaseIn()
            };

            t.Apply(filler, Avalonia.Animation.Clock.GlobalClock, 0d, (double)FlyoutWidth);

            // -10 is enough to avoid windows flashing
            await Task.Delay(AnimationDelay - 10);
            Close();
            Program.MainWindowInstance = null;

            _animating = false;
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            CloseAnimated();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void BtnDisplaySettings_OnClick(object? sender, RoutedEventArgs e)
        {
            Windows.Run("cmd", " /k start ms-settings:display && exit");
        }

        public static void CreateAndShow()
        {
            if (Program.MainWindowInstance == null)
            {
                Program.MainWindowInstance = new MainWindow();
                MainWindow flyout = Program.MainWindowInstance;
                flyout.DataContext = new DisplayControlViewModel();
            }
            Program.MainWindowInstance.ShowAnimated();
        }

        public static  void Preload()
        {
            var prelodWindow = new MainWindow();
            prelodWindow.DataContext = new DisplayControlViewModel();
            prelodWindow.Opacity = 0;
            prelodWindow.ShowAnimated();
            Thread.Sleep(500);
            prelodWindow.Close();
            Thread.Sleep(500);
        }
    }
}
