using System;
using System.Windows.Forms;

namespace DisplayControlFlyout.Services.IKeyboardHookServices
{
    public class KeyboardHookServices : IKeyboardHookServices, IDisposable
    {
        private readonly KeyboardHook _keyboardHook = new KeyboardHook();

        public KeyboardHookServices()
        {
            _keyboardHook.KeyPressed += _keyboardHook_KeyPressed;
        }

        private void _keyboardHook_KeyPressed(object? sender, KeyPressedEventArgs e)
        {
            KeyPressed?.Invoke(this, e);
        }

        public void RegisterHotKey(ModifierKeys modifier, Keys key)
        {
            _keyboardHook.RegisterHotKey(modifier, key);

        }
        public event EventHandler<KeyPressedEventArgs>? KeyPressed;

        public void Dispose()
        {
            _keyboardHook.Dispose();
        }
    }
}
