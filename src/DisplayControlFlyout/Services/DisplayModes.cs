using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DisplayControlFlyout.Extensions;
using DisplayControlFlyout.IoC;
using Humanizer;
using WindowsDisplayAPI.DisplayConfig;

namespace DisplayControlFlyout.Services
{
    public enum DisplayMode
    {
        Single,                                     // OK
        ExtendedHorizontal,                         // OK
        ExtendedAll,                                // OK
        ExtendedDuplicated,                         // OK
        Tv,                                         // OK
        ExtendedSingle,
        DuplicatedSingle,
        Unknown
    }

    public static class DisplayManager
    {
        public static void ShowToast(DisplayMode mode)
        {

            var n = Kernel.Get<INotificationServices>();
            n.Show("Display mode changed", $"New display mode is {mode.ToString().Humanize()}", mode.ToUriPackImage());
        }

        // Specific for my setup
        public static DisplayMode GetCurrentMode()
        {
            try
            {
                var paths = PathInfo.GetActivePaths();


                if (paths.Count(p => p.IsInUse) == 1 && paths[0].TargetsInfo.Count(i => i.DisplayTarget.FriendlyName == "VG27A") == 1 && paths[0].TargetsInfo.Count(i => i.DisplayTarget.FriendlyName == "LG TV") == 1)// Hay uno en uso con un monitor y la tv
                    return DisplayMode.DuplicatedSingle;

                if (paths.Count(p => p.IsInUse) == 3)
                    return DisplayMode.ExtendedAll;

                if (paths.Count(p => p.IsInUse) == 2 && paths.Count(p => p.IsCloneMember == false && p.IsInUse && p.TargetsInfo[0].DisplayTarget.FriendlyName == "VG27A" && p.IsInUse && p.TargetsInfo.Length == 1) == 2) //Hay dos en uso y cada uno tiene un único path que es un monitor
                    return DisplayMode.ExtendedHorizontal;

                if (paths.Count(p => p.IsInUse) == 1 && paths.Count(p => p.IsCloneMember == false && p.IsInUse && p.TargetsInfo[0].DisplayTarget.FriendlyName == "VG27A") == 1) // Hay uno en uso y es un monitor
                    return DisplayMode.Single;

                if (paths.Count(p => p.IsInUse) == 1 && paths.Count(p => p.IsCloneMember == false && p.IsInUse && p.TargetsInfo[0].DisplayTarget.FriendlyName == "LG TV") == 1) // Hay uno en uso y es la TV
                    return DisplayMode.Tv;

                if ((paths.Count(p => p.IsInUse) == 2 && paths.Count(p => p.IsCloneMember == false && p.IsInUse && p.TargetsInfo[0].DisplayTarget.FriendlyName == "VG27A") == 1) && (paths.Count(p => p.IsInUse) == 2 && paths.Count(p => p.IsCloneMember == false && p.IsInUse && p.TargetsInfo[0].DisplayTarget.FriendlyName == "LG TV") == 1))
                    return DisplayMode.ExtendedSingle;

                if (paths.Count(p => p.IsInUse) == 2 && paths.Count(p => p.IsCloneMember == false && p.IsInUse && p.TargetsInfo.Length == 2) == 1 && paths.Count(p => p.IsCloneMember == false && p.IsInUse && p.TargetsInfo.Length == 1) == 1) // Hay dos en uso y uno de los dos tiene dos monitores
                    return DisplayMode.ExtendedDuplicated;

                return DisplayMode.Unknown;

            }
            catch (Exception)
            {
                return DisplayMode.Unknown;
            }
        }

        public static async Task SetMode(string profileFileName, DisplayMode mode, bool useTv, bool showToast)
        {
            string monitorSwitcherPath = @"D:\Warez\Utiles\MonitorProfileSwitcher_v0700\MonitorSwitcher.exe";
            int expectedSuccessCount = 6;
            int modeChangeRetrySuccessDelay = 500;
            int modeChangeRetryFailDelay = 1000;
            int modeChangeRetryTimeout = 20000;

            int successCount = 0;
            Stopwatch timeoutDisplayModeWatch = new Stopwatch();
            timeoutDisplayModeWatch.Start();
            bool success = false;

            do
            {
                DisplayMode currentMode = DisplayManager.GetCurrentMode();

                if (currentMode != mode)
                {
                    successCount = 0;
                    Windows.Run(monitorSwitcherPath,
                        $@"""-load:C:\Users\cheer\AppData\Roaming\MonitorSwitcher\profiles\{profileFileName}""");
                }
                else
                {
                    successCount++;
                }
                if (successCount >= expectedSuccessCount)
                {
                    success = true;
                    if (showToast)
                        ShowToast(mode);
                    break;
                }
                await Task.Delay(successCount == 0 ? modeChangeRetryFailDelay : modeChangeRetrySuccessDelay);

            } while (timeoutDisplayModeWatch.ElapsedMilliseconds < modeChangeRetryTimeout);

            timeoutDisplayModeWatch.Stop();

            if (!success)
                return;

            for (int i = 0; i < 3; i++)
            {
                Television.SetPowerOnState(useTv);
                await Task.Delay(500);
            }
        }

        public static async Task SetMode(DisplayMode mode, bool showToast = true)
        {
            Stopwatch timeoutDisplayModeWatch = new Stopwatch();
            timeoutDisplayModeWatch.Start();

            switch (mode)
            {
                case DisplayMode.Single:
                    await SetMode("Single.xml", DisplayMode.Single, false, showToast);
                    break;
                case DisplayMode.ExtendedHorizontal:
                    await SetMode("Extended horizontal.xml", DisplayMode.ExtendedHorizontal, false, showToast);
                    break;
                case DisplayMode.ExtendedAll:
                    await SetMode("Extended all.xml", DisplayMode.ExtendedAll, true, showToast);
                    break;
                case DisplayMode.ExtendedDuplicated:
                    await SetMode("Extended horizontal duplicated vertical.xml", DisplayMode.ExtendedDuplicated, true, showToast);
                    break;
                case DisplayMode.ExtendedSingle:
                    await SetMode("Extended single.xml", DisplayMode.ExtendedSingle, true, showToast);
                    break;
                case DisplayMode.DuplicatedSingle:
                    await SetMode("Duplicated single.xml", DisplayMode.DuplicatedSingle, true, showToast);
                    break;
                case DisplayMode.Tv:
                    if (DisplayManager.GetCurrentMode() != DisplayMode.Tv)
                        await SetMode("TV.xml", DisplayMode.Tv, true, showToast);
                    break;
                case DisplayMode.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}