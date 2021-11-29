using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DisplayControlFlyout.Extensions;
using DisplayControlFlyout.Services;
using DisplayControlFlyout.Services.FlyoutServices;
using DisplayControlFlyout.Services.IMonitorServices;
using Humanizer;
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
                Disposable.Create(() => { }).DisposeWith(disposables);
            });

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();

            _applicableDisplayModes = new List<ApplicableDisplayMode>();

            foreach (var mode in Enum.GetValues<DisplayMode>().Where(m => m != DisplayMode.Unknown))
            {
                _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = mode.ToString().Humanize(), Mode = mode, Image = mode.ToBitMap() });
            }

            _monitors = new List<DisplayBrightViewModel>();
            _monitorService.Refresh();
            foreach (var monitor in _monitorService.GetMonitors())
            {
                _monitors.Add(new DisplayBrightViewModel(_monitorService, monitor));
            }
        }

        private List<DisplayBrightViewModel> _monitors;
        public List<DisplayBrightViewModel> Monitors => _monitors;

        public uint Bright
        {
            get => _monitorService.GetAverage();
            set => _monitorService.SetAll(value);
        }

        public double FlyoutHeight => GlobalHDR ? 580 : 580 + (50 * _monitors.Count) + 20;

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

        // ReSharper disable once InconsistentNaming
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
