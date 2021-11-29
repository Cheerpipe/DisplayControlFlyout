﻿using System;
using Avalonia.Controls;

namespace DisplayControlFlyout.Services
{
    public interface INotificationServices
    {
        void Show(string title, string content, Uri imageUri);
    }
}
