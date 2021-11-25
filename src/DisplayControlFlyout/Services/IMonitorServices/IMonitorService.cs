

namespace DisplayControlFlyout.Services.IMonitorServices
{
    public interface IMonitorService
    {
        void SetAll(uint bright);
        int GetAverage();
        void Refresh();
    }
}
