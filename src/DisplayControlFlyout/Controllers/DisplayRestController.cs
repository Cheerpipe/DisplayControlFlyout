using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using DisplayControlFlyout.Services;
using DisplayControlFlyout.Services.FlyoutServices;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
// ReSharper disable UnusedMember.Global

namespace ArtemisFlyout.Controllers
{
    public class DisplayRestController : WebApiController
    {
        private readonly IFlyoutService _flyoutService;
        public DisplayRestController(WindowsFlyoutService flyoutService)
        {
            _flyoutService = flyoutService;
        }

        [Route(HttpVerbs.Get, "/enable_hdr")]
        public async Task EnableHDR()
        {
            //Dispatcher.UIThread.Post(() => _flyoutService.Show());
            HDR.SetGlobalHDRState(true);
        }

        [Route(HttpVerbs.Get, "/disable_hdr")]
        public async Task DisableHDR()
        {
            //Dispatcher.UIThread.Post(() => _flyoutService.Show());
            HDR.SetGlobalHDRState(false);
        }

        [Route(HttpVerbs.Get, "/toggle_hdr")]
        public async Task ToggleHDR()
        {
            //Dispatcher.UIThread.Post(() => _flyoutService.Show());
            HDR.SetGlobalHDRState(!HDR.GetGlobalHDRState());
        }

        [Route(HttpVerbs.Get, "/Single")]
        public async Task Single()
        {
            await DisplayManager.SetMode(DisplayMode.Single,true);
        }

        [Route(HttpVerbs.Get, "/ExtendedHorizontal")]
        public async Task ExtendedHorizontal()
        {
            await DisplayManager.SetMode(DisplayMode.ExtendedHorizontal, true);
        }

        [Route(HttpVerbs.Get, "/ExtendedAll")]
        public async Task ExtendedAll()
        {
            await DisplayManager.SetMode(DisplayMode.ExtendedAll, true);
        }

        [Route(HttpVerbs.Get, "/ExtendedDuplicated")]
        public async Task ExtendedDuplicated()
        {
            await DisplayManager.SetMode(DisplayMode.ExtendedDuplicated, true);
        }

        [Route(HttpVerbs.Get, "/Tv")]
        public async Task Tv()
        {
            await DisplayManager.SetMode(DisplayMode.Tv, true);
        }

        [Route(HttpVerbs.Get, "/ExtendedSingle")]
        public async Task ExtendedSingle()
        {
            await DisplayManager.SetMode(DisplayMode.ExtendedSingle, true);
        }

        [Route(HttpVerbs.Get, "/DuplicatedSingle")]
        public async Task DuplicatedSingle()
        {
            await DisplayManager.SetMode(DisplayMode.DuplicatedSingle, true);
        }

        [Route(HttpVerbs.Get, "/show")]
        public async Task ShowFlyout()
        {
            Dispatcher.UIThread.Post(()=> _flyoutService.Show());
            HttpContext.Response.ContentType = "application/json";
            await using var writer = HttpContext.OpenResponseText(new UTF8Encoding(false));
            await writer.WriteAsync(string.Empty);
        }

        [Route(HttpVerbs.Get, "/close")]
        public async Task CloseFlyout()
        {
            Dispatcher.UIThread.Post(() => _flyoutService.CloseAndRelease());
            HttpContext.Response.ContentType = "application/json";
            await using var writer = HttpContext.OpenResponseText(new UTF8Encoding(false));
            await writer.WriteAsync(string.Empty);
        }
    }
}
