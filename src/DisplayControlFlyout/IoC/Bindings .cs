using ArtemisFlyout.Services;
using DisplayControlFlyout.Services.IMonitorServices;
using DisplayControlFlyout.Services.TrayIcon;
using Ninject.Modules;

namespace ArtemisFlyout.IoC
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<ITrayIconService>().To<TrayIconService>().InSingletonScope();
            Bind<IFlyoutService>().To<FlyoutService>().InSingletonScope();
            Bind<IMonitorService>().To<MonitorService>().InSingletonScope();
        }
    }
}