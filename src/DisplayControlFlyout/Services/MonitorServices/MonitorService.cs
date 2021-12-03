using System;
using System.Collections.Generic;
using System.Linq;
using DisplayControlFlyout.Native;

namespace DisplayControlFlyout.Services.MonitorServices
{
    public class MonitorService : IMonitorService, IDisposable
    {
        private readonly PhysicalMonitorController _controller = new PhysicalMonitorController();
        public void SetAll(uint bright)
        {
            _controller.SetAll(bright);
        }

        public void Set(uint bright, PhysicalMonitorController.MonitorInfo monitor)
        {
            _controller.Set(bright, monitor);
        }

        public uint GetAverage()
        {
            return _controller.GetAverage();
        }

        public uint Get(PhysicalMonitorController.MonitorInfo monitor)
        {
            return _controller.Get(monitor);
        }

        public void Refresh()
        {
            _controller.UpdateMonitors();
        }

        public List<PhysicalMonitorController.MonitorInfo> GetMonitors()
        {
            return _controller.Monitors.ToList();
        }

        public void Dispose()
        {
            _controller?.Dispose();
        }
    }
}
