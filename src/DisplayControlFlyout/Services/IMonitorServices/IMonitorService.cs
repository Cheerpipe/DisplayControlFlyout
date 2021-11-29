

using System;
using System.Collections.Generic;
using DisplayControlFlyout.Native;

namespace DisplayControlFlyout.Services.IMonitorServices
{
    public interface IMonitorService
    {
        void SetAll(uint bright);
        void Set(uint bright, PhysicalMonitorBrightnessController.MonitorInfo monitor);
        uint GetAverage();
        uint Get(PhysicalMonitorBrightnessController.MonitorInfo monitor);
        void Refresh();
        List<PhysicalMonitorBrightnessController.MonitorInfo> GetMonitors();
    }
}
