using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayControlFlyout.ViewModels
{
    public class DisplayBrightViewModel
    {
        public IntPtr Id { get; set; }
        public uint Brightness { get; set; }

        public DisplayBrightViewModel(IntPtr id, uint brightness)
        {
            Id=id;
            Brightness=brightness;
        }
    }
}
