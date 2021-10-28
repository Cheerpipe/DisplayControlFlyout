using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DisplayControlFlyout.UserControls
{
    public partial class DisplayControl : UserControl
    {
        public DisplayControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            //this.DataContext = new DisplayControlViewModel();
            AvaloniaXamlLoader.Load(this);
        }

        private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (Program.MainWindowInstance.IsVisible)
                Program.MainWindowInstance.CloseAnimated();
        }
    }
}
