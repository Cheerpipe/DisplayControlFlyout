using System;
using System.Threading;

namespace DisplayControlFlyout.Services
{
    public class InstanceService : IInstanceService, IDisposable
    {
        public Mutex InstanceMutex { get; private set; }

        public bool IsAlreadyRunning()
        {
            InstanceMutex = new Mutex(true, "99a90979-eb56-45e5-adec-398a348c2d9c", out bool created);
            return !created;
        }

        public void Dispose()
        {
            InstanceMutex?.Dispose();
        }
    }
}
