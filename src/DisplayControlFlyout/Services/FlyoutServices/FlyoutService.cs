using System;
using System.Threading.Tasks;
using DisplayControlFlyout.IoC;
using DisplayControlFlyout.ViewModels;
using DisplayControlFlyout.Views;
using Ninject;

namespace DisplayControlFlyout.Services.FlyoutServices
{
    public class FlyoutService : IFlyoutService
    {
        public static FlyoutContainer? FlyoutWindowInstance { get; private set; }
        private readonly IKernel _kernel;
        private bool _opening;
        private bool _closing;

        public FlyoutService(IKernel kernel)
        {
            _kernel = kernel;
        }


        public async void Show(bool animate = true)
        {
            if (_opening)
                return;

            _opening = true;

            if (FlyoutWindowInstance != null) return;
            FlyoutWindowInstance = GetInstance();

            FlyoutWindowInstance.Deactivated += async (_, _) =>
            {
                await CloseAndRelease();
            };

            if (animate)
                await FlyoutWindowInstance.ShowAnimated();
            else
                FlyoutWindowInstance.Show();
            _opening = false;
        }

        public void SetHeight(double newHeight)
        {
            FlyoutWindowInstance?.SetHeight(newHeight);
        }

        public void SetWidth(double newWidth)
        {
            FlyoutWindowInstance?.SetWidth(newWidth);
        }

        //TODO: Move ViewModel creation outside the Service
        private FlyoutContainer GetInstance()
        {

            FlyoutContainer flyoutInstance = _kernel.Get<FlyoutContainer>();
            flyoutInstance.DataContext = Kernel.Get<FlyoutContainerViewModel>();
            return flyoutInstance;
        }

        public async Task Preload()
        {
            if (FlyoutWindowInstance != null) return;
            FlyoutWindowInstance = GetInstance();
            await FlyoutWindowInstance.ShowAnimated(true);
            await Task.Delay(300);
            await CloseAndRelease(false);
        }

        public async void Toggle()
        {
            if (FlyoutWindowInstance is null)
            {
                Show();
            }
            else
            {
                await CloseAndRelease();
            }
        }

        public async Task CloseAndRelease(bool animate = true)
        {
            if (_closing)
                return;
            _closing = true;

            if (animate)
                await FlyoutWindowInstance!.CloseAnimated();
            else
                FlyoutWindowInstance!.Close();

            FlyoutWindowInstance = null;

            _closing = false;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
