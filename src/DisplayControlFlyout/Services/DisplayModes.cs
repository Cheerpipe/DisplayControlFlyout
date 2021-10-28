using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsDisplayAPI.DisplayConfig;

namespace DisplayControlFlyout.Services
{
    public enum DisplayMode
    {
        Single,                                     // OK
        ExtendedHorizontal,                         // OK
        ExtendedAll,                                // OK
        ExtendedHorizontalDuplicatedVertical,       // OK
        Tv,                                         // OK
        ExtendedSingle,
        DuplicatedSingle,
        Unknown
    }

    public static class DisplayManager
    {
        private const string MonitorSwitcherPath = @"D:\Warez\Utiles\MonitorProfileSwitcher_v0700\MonitorSwitcher.exe";
        private const int SetNewModeTimeout = 20;
        private const int SetNewModeSucessCount = 5;
        public static DisplayMode GetCurrentMode()
        {
            try
            {
                var paths = PathInfo.GetActivePaths();

                if (paths.Where(p => p.IsInUse).Count() == 3) // Hay tres en uso
                    return DisplayMode.ExtendedAll;

                if (paths.Where(p => p.IsInUse).Count() == 2 && paths.Where(p => p.IsCloneMember == false && p.IsInUse && p.TargetsInfo[0].DisplayTarget.FriendlyName == "VG27A" && p.IsInUse && p.TargetsInfo.Length == 1).Count() == 2) //Hay dos en uso y cada uno tiene un único path que es un monitor
                    return DisplayMode.ExtendedHorizontal;

                if (paths.Where(p => p.IsInUse).Count() == 1 && paths.Where(p => p.IsCloneMember == false && p.IsInUse && p.TargetsInfo[0].DisplayTarget.FriendlyName == "VG27A").Count() == 1) // Hay uno en uso y es un monitor
                    return DisplayMode.Single;

                if (paths.Where(p => p.IsInUse).Count() == 1 && paths.Where(p => p.IsCloneMember == false && p.IsInUse && p.TargetsInfo[0].DisplayTarget.FriendlyName == "LG TV").Count() == 1) // Hay uno en uso y es la TV
                    return DisplayMode.Tv;

                if (paths.Where(p => p.IsInUse).Count() == 2 && paths.Where(p => p.IsCloneMember == false && p.IsInUse && p.TargetsInfo.Length == 2).Count() == 1 && paths.Where(p => p.IsCloneMember == false && p.IsInUse && p.TargetsInfo.Length == 1).Count() == 1) // Hay dos en uso y uno de los dos tiene dos monitores
                    return DisplayMode.ExtendedHorizontalDuplicatedVertical;

                return DisplayMode.Unknown;

            }
            catch (Exception)
            {
                return DisplayMode.Unknown;
            }
        }

        public async static void SetMode(DisplayMode mode)
        {
            int maxRetry = 10;
            int retryCount = 0;
            int secondsCount = 0;
            int successCount = 0;

            switch (mode)
            {
                case DisplayMode.Single:
                    if (DisplayManager.GetCurrentMode() != DisplayMode.Single)
                        do
                        {
                            Windows.Run(MonitorSwitcherPath, @"""-load:C:\Users\cheer\AppData\Roaming\MonitorSwitcher\profiles\Single.xml""");
                            retryCount++;
                            await Task.Delay(1000);
                        } while (DisplayManager.GetCurrentMode() != DisplayMode.Single && retryCount < maxRetry);
                    //ShowToast(DisplayMode.Single);
                    if (retryCount < maxRetry)
                        for (int i = 1; i < 3; i++)
                        {
                            Television.SetPowerOnState(false);
                            Thread.Sleep(500);
                        }
                    break;
                case DisplayMode.ExtendedHorizontal:
                    do
                    {
                        DisplayMode currentMode = DisplayManager.GetCurrentMode();

                        if (currentMode != DisplayMode.ExtendedHorizontal)
                        {
                            successCount = 0;
                            Windows.Run(MonitorSwitcherPath,
                                @"""-load:C:\Users\cheer\AppData\Roaming\MonitorSwitcher\profiles\Extended horizontal.xml""");
                            retryCount++;
                        }
                        else
                        {
                            successCount++;
                        }

                        if (successCount == SetNewModeSucessCount)
                        {
                            //Success?
                            //ShowToast(currentMode);
                            break;
                        }

                        if (secondsCount > SetNewModeTimeout)
                        {
                            // Failed
                            //ShowFailedChangeToast(DisplayMode.ExtendedHorizontal);
                            return;
                        }

                        await Task.Delay(1000);
                        secondsCount++;

                    } while (true);

                    if (retryCount < maxRetry)
                        for (int i = 1; i < 3; i++)
                        {
                            Television.SetPowerOnState(false);
                            await Task.Delay(500);
                        }
                    break;
                case DisplayMode.ExtendedAll:
                    if (DisplayManager.GetCurrentMode() != DisplayMode.ExtendedAll)
                        do
                        {
                            Windows.Run(MonitorSwitcherPath, @"""-load:C:\Users\cheer\AppData\Roaming\MonitorSwitcher\profiles\Extended all.xml""");
                            retryCount++;
                            await Task.Delay(1000);
                        } while (DisplayManager.GetCurrentMode() != DisplayMode.ExtendedAll && retryCount < maxRetry);
                    //ShowToast(DisplayMode.ExtendedAll);

                    if (retryCount < maxRetry)
                    {
                        Television.SetInputBByIndex(1);
                        for (int i = 1; i < 3; i++)
                        {
                            Television.SetPowerOnState(true);
                            Thread.Sleep(500);
                        }
                    }
                    break;
                case DisplayMode.ExtendedHorizontalDuplicatedVertical:
                    if (DisplayManager.GetCurrentMode() != DisplayMode.ExtendedHorizontalDuplicatedVertical)
                        do
                        {
                            Windows.Run(MonitorSwitcherPath, @"""-load:C:\Users\cheer\AppData\Roaming\MonitorSwitcher\profiles\Extended horizontal duplicated vertical.xml""");
                            retryCount++;
                            await Task.Delay(1000);
                        } while (DisplayManager.GetCurrentMode() != DisplayMode.ExtendedHorizontalDuplicatedVertical && retryCount < maxRetry);
                    //ShowToast(DisplayMode.ExtendedHorizontalDuplicatedVertical);

                    if (retryCount < maxRetry)
                    {
                        Television.SetInputBByIndex(1);
                        for (int i = 1; i < 3; i++)
                        {
                            Television.SetPowerOnState(true);
                            Thread.Sleep(500);
                        }
                    }
                    break;
                case DisplayMode.Tv:
                    if (DisplayManager.GetCurrentMode() != DisplayMode.Tv)
                        Windows.Run(@"D:\Warez\Utiles\MonitorProfileSwitcher_v0700\MonitorSwitcher.exe", @"""-load:C:\Users\cheer\AppData\Roaming\MonitorSwitcher\profiles\TV.xml""");
                    // ShowToast(DisplayMode.Tv);
                    for (int i = 1; i < 3; i++)
                    {
                        Television.SetPowerOnState(true);
                        Thread.Sleep(500);
                    }
                    break;
                case DisplayMode.DuplicatedSingle:
                case DisplayMode.ExtendedSingle:
                    break;
                default:
                    break;
            }
        }
    }
}
