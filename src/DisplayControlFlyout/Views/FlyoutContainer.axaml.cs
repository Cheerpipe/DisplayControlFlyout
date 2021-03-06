using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.ReactiveUI;
using DisplayControlFlyout.Platform.Windows;
using DisplayControlFlyout.ViewModels;
using ReactiveUI;

// ReSharper disable UnusedParameter.Local

namespace DisplayControlFlyout.Views
{
    public class FlyoutContainer : ReactiveWindow<FlyoutContainerViewModel>
    {
        public FlyoutContainer()
        {
            this.WhenActivated(disposables =>
            {
                /* Handle interactions etc. */
            });
            AvaloniaXamlLoader.Load(this);

#if DEBUG
            this.AttachDevTools();:
#endif

            _screenWidth = Screens.Primary.WorkingArea.Width;
            _screenHeight = Screens.Primary.WorkingArea.Height;
        }

        private readonly int _screenHeight;
        private readonly int _screenWidth;

        public int ShowAnimationDelay { get; set; } = 250;
        public int ContentRevealAnimationDelay { get; set; } = 1000;
        public int CloseAnimationDelay { get; set; } = 200;
        public int ResizeAnimationDelay { get; set; } = 200;
        public int FlyoutSpacing { get; set; } = 12;


        public async Task ShowAnimated()
        {
            PointerPressed += FlyoutPanelContainer_PointerPressed;
            PointerReleased += FlyoutPanelContainer_PointerReleased;
            PointerMoved += FlyoutPanelContainer_PointerMoved;

            PropertyChanged += FlyoutWindow_PropertyChanged;

            WindowStartupLocation = WindowStartupLocation.Manual;
            Width = 400;
            //Height = 650;

            Position = new PixelPoint(_screenWidth - (int)(Width + 12), Position.Y);

            Show();

            Clock = Avalonia.Animation.Clock.GlobalClock;
            IntegerTransition showTransition = new IntegerTransition()
            {
                Property = FlyoutContainer.VerticalPositionProperty,
                Duration = TimeSpan.FromMilliseconds(ShowAnimationDelay),
                Easing = new ExponentialEaseOut()
            };

            showTransition.Apply(this, Avalonia.Animation.Clock.GlobalClock, _screenHeight, GetTargetVerticalPosition());

            Panel mainContainerPanel = this.Find<Panel>("MainContainerPanel");
            TransformOperationsTransition marginTransition = new TransformOperationsTransition()
            {
                Property = FlyoutContainer.RenderTransformProperty,
                Duration = TimeSpan.FromMilliseconds(ContentRevealAnimationDelay),
                Easing = new ExponentialEaseOut()
            };
            marginTransition.Apply(mainContainerPanel, Avalonia.Animation.Clock.GlobalClock, TransformOperations.Parse("translate(-20px, 0px)"), TransformOperations.Parse("translate(0px, 0px)"));

            await Task.Delay(ShowAnimationDelay);
        }

        #region Drag to move
        private async void FlyoutPanelContainer_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isOnDrag = false;
            if (VerticalPosition >= GetTargetVerticalPosition() + GetTargetVerticalPosition() * 0.1f)
                await CloseAnimated(CloseAnimationDelay);
            else
                VerticalPosition = GetTargetVerticalPosition();
        }

        private double _previousPosition;
        private double _currentPosition;
        private void FlyoutPanelContainer_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (!_isOnDrag)
            {
                _previousPosition = e.GetPosition(this).Y;
                return;
            }

            if (e.Pointer.IsPrimary)
            {
                _currentPosition = this.PointToScreen(e.GetPosition(this)).Y;
                double delta = _previousPosition - _currentPosition;
                _previousPosition = _currentPosition;

                if (VerticalPosition <= GetTargetVerticalPosition() && delta > 0)
                {
                    VerticalPosition = GetTargetVerticalPosition();
                    return;
                }


                VerticalPosition -= (int)delta;
            }
        }

        public int GetTargetVerticalPosition() => _screenHeight - (int)(Height + FlyoutSpacing);

        private bool _isOnDrag;
        private void FlyoutPanelContainer_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!e.Pointer.IsPrimary) return;

            switch (e.Source)
            {
                case Border border when (border.TemplatedParent is ComboBox) || (border.TemplatedParent is ComboBoxItem):
                case TextBlock:
                    return;
                default:
                    _previousPosition = this.PointToScreen(e.GetPosition(this)).Y;
                    _isOnDrag = true;
                    break;
            }
        }

        #endregion

        public async Task CloseAnimated(double animationDuration)
        {
            var closeTransition = new IntegerTransition()
            {
                Property = FlyoutContainer.VerticalPositionProperty,
                Duration = TimeSpan.FromMilliseconds(animationDuration),
                Easing = new CircularEaseIn(),
            };

            Activate();
            closeTransition.Apply(this, Avalonia.Animation.Clock.GlobalClock, VerticalPosition, _screenHeight);
            await Task.Delay(CloseAnimationDelay);
            Close();
        }

        public async Task CloseAnimated()
        {
            await CloseAnimated(CloseAnimationDelay);
        }

        public void SetHeight(double newHeight)
        {
            var heightTransition = new DoubleTransition()
            {
                Property = HeightProperty,
                Duration = TimeSpan.FromMilliseconds(ResizeAnimationDelay),
                Easing = new CircularEaseOut()
            };

            heightTransition.Apply(this, Avalonia.Animation.Clock.GlobalClock, Height, newHeight);
        }

        public void SetWidth(double newWidth)
        {
            var widthTransition = new DoubleTransition()
            {
                Property = WidthProperty,
                Duration = TimeSpan.FromMilliseconds(ResizeAnimationDelay),
                Easing = new CircularEaseOut()
            };

            widthTransition.Apply(this, Avalonia.Animation.Clock.GlobalClock, Width, newWidth);
        }

        private void FlyoutWindow_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Height" || e.Property.Name == "Width" || e.Property.Name == "HorizontalPosition")
            {
                Position = new PixelPoint(_screenWidth + (-(int)Width), _screenHeight + (-(int)Height));
            }
        }
        public static readonly AttachedProperty<int> HorizontalPositionProperty = AvaloniaProperty.RegisterAttached<FlyoutContainer, Control, int>(nameof(HorizontalPosition));

        public int HorizontalPosition
        {
            get => GetValue(HorizontalPositionProperty);
            set
            {
                SetValue(HorizontalPositionProperty, value);
                RenderTransform = new TranslateTransform(value, 0);
            }
        }

        public static readonly AttachedProperty<int> VerticalPositionProperty = AvaloniaProperty.RegisterAttached<FlyoutContainer, Control, int>(nameof(VerticalPosition));

        public int VerticalPosition
        {
            get => GetValue(VerticalPositionProperty);
            set
            {
                if (PlatformImpl != null)
                {
                    NativeMethods.SetWindowRgn(PlatformImpl.Handle.Handle, NativeMethods.CreateRectRgn(0, 0, (int)Width, _screenHeight - value), true);
                }

                SetValue(VerticalPositionProperty, value);
                Position = new PixelPoint(Position.X, value);
            }
        }
        static FlyoutContainer()
        {
            HorizontalPositionProperty.Changed.Subscribe(HorizontalPositionChanged);
            VerticalPositionProperty.Changed.Subscribe(VerticalPositionChanged);
        }

        private static void HorizontalPositionChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var flyoutContainer = (FlyoutContainer)e.Sender;
            var newPositionValue = (int)e.NewValue!;
            flyoutContainer.HorizontalPosition = newPositionValue;
        }

        private static void VerticalPositionChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var flyoutContainer = (FlyoutContainer)e.Sender;
            var newPositionValue = (int)e.NewValue!;
            flyoutContainer.VerticalPosition = newPositionValue;
        }

        // ReSharper disable once UnusedMember.Local
        private void BtnDisplaySettings_OnClick(object sender, RoutedEventArgs e)
        {
            Services.Windows.Run("cmd", " /k start ms-settings:display && exit");
        }
    }
}
