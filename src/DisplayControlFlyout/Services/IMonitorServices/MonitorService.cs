using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace DisplayControlFlyout.Services.IMonitorServices
{
    public class MonitorService : IMonitorService, IDisposable
    {
        private readonly PhysicalMonitorBrightnessController _brightnessController = new PhysicalMonitorBrightnessController();
        public void SetAll(uint bright)
        {
            _brightnessController.SetAll(bright);
        }

        public int GetAverage()
        {
            return _brightnessController.Get();
        }

        public void Refresh()
        {
            _brightnessController.UpdateMonitors();
        }

        public void Dispose()
        {
            _brightnessController?.Dispose();
        }
    }
}
