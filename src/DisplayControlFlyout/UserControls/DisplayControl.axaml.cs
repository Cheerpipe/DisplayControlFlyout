using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using DisplayControlFlyout.IoC;
using DisplayControlFlyout.Services;
using DisplayControlFlyout.Services.FlyoutServices;
using DisplayControlFlyout.ViewModels;
using FluentAvalonia.Core;


namespace DisplayControlFlyout.UserControls
{
    public class DisplayControl : UserControl
    {
        private readonly ListBox _displayModeRepeater;
        public DisplayControl()
        {
            InitializeComponent();
            _displayModeRepeater = this.Find<ListBox>("DisplayModeRepeater");
            _displayModeRepeater.KeyDown += _displayModeRepeater_KeyDown;
            _displayModeRepeater.Tapped += _displayModeRepeater_Tapped;
            _displayModeRepeater.EffectiveViewportChanged += _displayModeRepeater_EffectiveViewportChanged;
        }

        private void _displayModeRepeater_EffectiveViewportChanged(object? sender, Avalonia.Layout.EffectiveViewportChangedEventArgs e)
        {
            if (_displayModeRepeater.Items == null) return;

            ListBoxItem focusedItem = (_displayModeRepeater.ItemContainerGenerator.ContainerFromIndex(_displayModeRepeater.Items.IndexOf(_displayModeRepeater.SelectedItem)) as ListBoxItem)!;
            focusedItem.Focus();
        }

        private void _displayModeRepeater_Tapped(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ApplyCurrentSelectedMode();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void _displayModeRepeater_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Space) return;
            ApplyCurrentSelectedMode();
        }

        private void ApplyCurrentSelectedMode()
        {
            List<ListBoxItem> itemContainers = _displayModeRepeater.ItemContainerGenerator.Containers.Select(p => p.ContainerControl).Cast<ListBoxItem>().ToList();
            _displayModeRepeater.SelectedItem = itemContainers.FirstOrDefault(ic => ic.IsFocused)?.DataContext;
            DisplayManager.SetMode((((ApplicableDisplayMode)_displayModeRepeater.SelectedItem!)).Mode);
            Kernel.Get<IFlyoutService>().CloseAndRelease();
        }
    }
}
