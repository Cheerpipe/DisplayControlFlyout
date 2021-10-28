using System;
using System.Collections.Generic;
using System.Linq;
using AutoHDR.Displays;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DisplayControlFlyout.Services;

namespace DisplayControlFlyout.ViewModels
{
    public class DisplayControlViewModel : ViewModelBase
    {
        private List<ApplicableDisplayMode> _applicableDisplayModes;
        public DisplayControlViewModel()
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();

            _applicableDisplayModes = new List<ApplicableDisplayMode>();
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Single", Mode = DisplayMode.Single, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.single.png"))) });
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Extended to horizontal", Mode = DisplayMode.ExtendedHorizontal, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.extended_horizontal.png"))) });
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Extended to all", Mode = DisplayMode.ExtendedAll, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.extended_all.png"))) });
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Duplicated vertical", Mode = DisplayMode.ExtendedHorizontalDuplicatedVertical, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.extendeds_plus_duplicated.png"))) });
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Television only", Mode = DisplayMode.Tv, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.tv.png"))) });
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Duplicated single", Mode = DisplayMode.DuplicatedSingle, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.duplicated_single.png"))) });
            _applicableDisplayModes.Add(new ApplicableDisplayMode { DisplayName = "Extended single", Mode = DisplayMode.ExtendedSingle, Image = new Bitmap(assets.Open(new Uri(@"resm:DisplayControlFlyout.Assets.extended_single.png"))) });
        }

        public List<ApplicableDisplayMode> ApplicableDisplayModes
        {
            get => _applicableDisplayModes;
        }

        public ApplicableDisplayMode SelectedApplicableDisplayMode
        {
            get
            {
                DisplayMode currentDisplayMode = DisplayManager.GetCurrentMode();
                ApplicableDisplayMode currentSelectedMode = _applicableDisplayModes.FirstOrDefault(i => i.Mode == currentDisplayMode);
                return currentSelectedMode;
            }
            set
            {
                DisplayManager.SetMode(value.Mode);
            }
        }

        public bool GlobalHDR
        {
            get
            {
                return HDR.GetGlobalHDRState();
            }
            set
            {
                HDR.SetGlobalHDRState(value);
            }
        }

        public bool Television
        {
            get
            {
                return Services.Television.IsOn;
            }
            set
            {
                Services.Television.SetPowerOnState(value);
            }

        }
    }
}
