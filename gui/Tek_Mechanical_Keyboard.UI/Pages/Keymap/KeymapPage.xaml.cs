using System;
using Tek_Mechanical_Keyboard.UI.Pages.Keymap.ViewModels;

namespace Tek_Mechanical_Keyboard.UI.Pages.Keymap;

/// <summary>
/// View only. All KeyMap behavior is implemented by KeyMapViewModel commands and services.
/// </summary>
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
}
