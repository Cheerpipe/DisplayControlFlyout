using DisplayControlFlyout.Platform.Windows;
using DisplayControlFlyout.Services.MonitorServices;

namespace DisplayControlFlyout.ViewModels
{
    public class DisplayBrightViewModel
    {
        private readonly IMonitorService _monitorService;

        public DisplayBrightViewModel(IMonitorService monitorService, PhysicalMonitorController.MonitorInfo monitor)
        {
            _monitorService = monitorService;
            Monitor = monitor;
        }

        public PhysicalMonitorController.MonitorInfo Monitor { get; init; }

        public uint Bright
        {
            get => _monitorService.Get(Monitor);
            set => _monitorService.Set(value, Monitor);
        }

        public string Name { get; init; }
    }
}
