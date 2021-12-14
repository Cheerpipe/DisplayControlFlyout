using Avalonia.Controls.Notifications;
using DisplayControlFlyout.Services;
using DisplayControlFlyout.Services.FlyoutServices;
using DisplayControlFlyout.Services.IKeyboardHookServices;
using DisplayControlFlyout.Services.MonitorServices;
using DisplayControlFlyout.Services.TrayIcon;
using Ninject.Modules;

namespace DisplayControlFlyout.IoC
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<ITrayIconService>().To<WindowsTrayIconService>().InSingletonScope();
            Bind<IFlyoutService>().To<FlyoutService>().InSingletonScope();
            Bind<IMonitorService>().To<MonitorService>().InSingletonScope();
            Bind<INotificationServices>().To<WindowsNotificationService>().InSingletonScope();
            Bind<IKeyboardHookServices>().To<KeyboardHookServices>().InSingletonScope();
            Bind<IInstanceService>().To<InstanceService>().InSingletonScope();
        }
    }
}