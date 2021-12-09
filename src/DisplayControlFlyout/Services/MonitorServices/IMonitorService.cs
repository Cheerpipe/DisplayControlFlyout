using System.Collections.Generic;
using DisplayControlFlyout.Platform.Windows;

namespace DisplayControlFlyout.Services.MonitorServices
{
    public interface IMonitorService
    {
        void SetAll(uint bright);
        void Set(uint bright, PhysicalMonitorController.MonitorInfo monitor);
        uint GetAverage();
        uint Get(PhysicalMonitorController.MonitorInfo monitor);
        void Refresh();
        List<PhysicalMonitorController.MonitorInfo> GetMonitors();
    }
}
