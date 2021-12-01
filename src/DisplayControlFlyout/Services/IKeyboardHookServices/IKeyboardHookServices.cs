using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DisplayControlFlyout.Services.IKeyboardHookServices
{
    public interface IKeyboardHookServices
    {
        public event EventHandler<KeyPressedEventArgs> KeyPressed;
        public void RegisterHotKey(ModifierKeys modifier, Keys key);
    }
}
