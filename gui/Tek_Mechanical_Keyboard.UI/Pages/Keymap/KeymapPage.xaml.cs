using System;
using Tek_Mechanical_Keyboard.UI.Pages.Keymap.ViewModels;

namespace Tek_Mechanical_Keyboard.UI.Pages.Keymap;
public partial class KeymapPage : BasePage
{
    public KeymapPage(KeyMapViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }

    private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {

    }

    private void KeyboardLayout_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {

    }
}
