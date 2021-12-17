using System;
using System.Windows.Forms;

namespace DisplayControlFlyout.Services.IKeyboardHookServices
{
    public interface IKeyboardHookServices
    {
        public event EventHandler<KeyPressedEventArgs> KeyPressed;
        public void RegisterHotKey(ModifierKeys modifier, Keys key);
    }
}
