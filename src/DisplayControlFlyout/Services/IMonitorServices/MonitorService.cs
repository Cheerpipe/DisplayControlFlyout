using System;
using System.Collections.Generic;
using System.Linq;

namespace DisplayControlFlyout.Services.IMonitorServices
{
    public class MonitorService : IMonitorService, IDisposable
    {
        private readonly PhysicalMonitorBrightnessController _brightnessController = new PhysicalMonitorBrightnessController();
        public void SetAll(uint bright)
        {
            _brightnessController.SetAll(bright);
        }

        public void Set(uint bright, PhysicalMonitorBrightnessController.MonitorInfo monitor)
        {
            _brightnessController.Set(bright, monitor);
        }

        public uint GetAverage()
        {
            return _brightnessController.GetAverage();
        }

        public uint Get(PhysicalMonitorBrightnessController.MonitorInfo monitor)
        {
            return _brightnessController.Get(monitor);
        }

        public void Refresh()
        {
            _brightnessController.UpdateMonitors();
        }

        public List<PhysicalMonitorBrightnessController.MonitorInfo> GetMonitors()
        {
            return _brightnessController.Monitors.ToList();
        }

        public void Dispose()
        {
            _brightnessController?.Dispose();
        }
    }
}
