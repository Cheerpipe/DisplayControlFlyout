using DisplayControlFlyout.Services.IMonitorServices;

namespace DisplayControlFlyout.ViewModels
{
    public class DisplayBrightViewModel
    {
        private readonly IMonitorService _monitorService;

        public DisplayBrightViewModel(IMonitorService monitorService, PhysicalMonitorBrightnessController.MonitorInfo monitor)
        {
            _monitorService = monitorService;
            Monitor = monitor;
        }

        public PhysicalMonitorBrightnessController.MonitorInfo Monitor { get; init; }

        public uint Bright
        {
            get => _monitorService.Get(Monitor);
            set => _monitorService.Set(value, Monitor);
        }

        public string Name { get; init; }
    }
}
