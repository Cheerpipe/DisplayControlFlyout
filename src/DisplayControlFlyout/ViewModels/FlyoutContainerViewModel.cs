using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using DisplayControlFlyout.Extensions;
using DisplayControlFlyout.Native;
using DisplayControlFlyout.Services;
using DisplayControlFlyout.Services.FlyoutServices;
using DisplayControlFlyout.Services.MonitorServices;
using Humanizer;
using ReactiveUI;

namespace DisplayControlFlyout.ViewModels
{
    public class FlyoutContainerViewModel : ViewModelBase
    {
        private readonly List<ApplicableDisplayMode?> _applicableDisplayModes;
        private readonly IFlyoutService _flyoutService;
        private readonly IMonitorService _monitorService;
        private ApplicableDisplayMode? _currentDisplayMode;
        public FlyoutContainerViewModel(IMonitorService monitorService, IFlyoutService flyoutService)
        {
            _monitorService = monitorService;
            _flyoutService = flyoutService;

            this.WhenActivated(disposables =>
            {
                Disposable.Create(() => { }).DisposeWith(disposables);
            });

            _applicableDisplayModes = new List<ApplicableDisplayMode?>();

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

            _currentDisplayMode = _applicableDisplayModes.FirstOrDefault(i => i != null && i.Mode == DisplayManager.GetCurrentMode());

        }

        private List<DisplayBrightViewModel> _monitors;
        public List<DisplayBrightViewModel> Monitors => _monitors;

        public uint Bright
        {
            get => _monitorService.GetAverage();
            set => _monitorService.SetAll(value);
        }

        public double FlyoutHeight => GlobalHDR ? 560 : 560 + (55 * _monitors.Count) + 20;

        public List<ApplicableDisplayMode?> ApplicableDisplayModes => _applicableDisplayModes;
        public ApplicableDisplayMode? SelectedApplicableDisplayMode
        {
            get => _currentDisplayMode;
            set => this.RaiseAndSetIfChanged(ref _currentDisplayMode, value);
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

        public void TurnOffDisplays()
        {
            PhysicalMonitorController.TurnDisplayOff();
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
