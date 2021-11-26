using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using ArtemisFlyout.Services;
using AutoHDR.Displays;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DisplayControlFlyout.Services;
using DisplayControlFlyout.Services.IMonitorServices;
using ReactiveUI;

namespace DisplayControlFlyout.ViewModels
{
    public class FlyoutContainerViewModel : ViewModelBase
    {
        private readonly List<ApplicableDisplayMode> _applicableDisplayModes;
        private readonly IFlyoutService _flyoutService;
        private readonly IMonitorService _monitorService;
        public FlyoutContainerViewModel(IMonitorService monitorService, IFlyoutService flyoutService)
        {
            _monitorService = monitorService;
            _flyoutService = flyoutService;
            this.WhenActivated(disposables =>
            {
                _monitorService.Refresh();
                Disposable.Create(() => { }).DisposeWith(disposables);
            });

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            _applicableDisplayModes = new List<ApplicableDisplayMode>();
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Single", Mode = DisplayMode.Single, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.single.png"))) });
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Extended to horizontal", Mode = DisplayMode.ExtendedHorizontal, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.extended_horizontal.png"))) });
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Extended to all", Mode = DisplayMode.ExtendedAll, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.extended_all.png"))) });
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Duplicated vertical", Mode = DisplayMode.ExtendedHorizontalDuplicatedVertical, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.extendeds_plus_duplicated.png"))) });
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Television only", Mode = DisplayMode.Tv, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.tv.png"))) });
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Duplicated single", Mode = DisplayMode.DuplicatedSingle, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.duplicated_single.png"))) });
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Extended single", Mode = DisplayMode.ExtendedSingle, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.extended_single.png"))) });
            Thread.Sleep(100);
        }

        public int Bright
        {
            get => _monitorService.GetAverage();
            set => _monitorService.SetAll((uint)value);
        }

        public double FlyoutHeight
        {
            get
            {
                return GlobalHDR ? 580 : 650;
            }
        }


        public List<ApplicableDisplayMode> ApplicableDisplayModes => _applicableDisplayModes;

        public ApplicableDisplayMode SelectedApplicableDisplayMode
        {
            get
            {
                DisplayMode currentDisplayMode = DisplayManager.GetCurrentMode();
                ApplicableDisplayMode currentSelectedMode = _applicableDisplayModes.FirstOrDefault(i => i.Mode == currentDisplayMode);
                return currentSelectedMode;
            }
            set
            {
                _flyoutService.CloseAndRelease();
                DisplayManager.SetMode(value.Mode);
            }
        }

        public bool GlobalHDR
        {
            get => HDR.GetGlobalHDRState();
            set
            {
                _flyoutService.CloseAndRelease();
                HDR.SetGlobalHDRState(value);
            }
        }

        public bool Television
        {
            get => Services.Television.IsOn;
            set
            {
                _flyoutService.CloseAndRelease();
                Services.Television.SetPowerOnState(value);
            }

        }
    }
}
