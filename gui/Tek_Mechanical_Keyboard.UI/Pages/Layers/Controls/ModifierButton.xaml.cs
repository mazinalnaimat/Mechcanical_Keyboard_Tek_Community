using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Tek_Mechanical_Keyboard.UI.Pages.Layers.Controls;

public partial class ModifierButton : UserControl
{
    public static readonly DependencyProperty SelectCommandProperty = DependencyProperty.Register(
        nameof(SelectCommand),
        typeof(ICommand),
        typeof(ModifierButton),
        new PropertyMetadata(null));

    public ModifierButton() => InitializeComponent();

    public ICommand? SelectCommand
    {
        get => (ICommand?)GetValue(SelectCommandProperty);
        set => SetValue(SelectCommandProperty, value);
    }
}
