using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Tek_Mechanical_Keyboard.UI.Pages.Layers.Controls;

public partial class KeyButton : UserControl
{
    public static readonly DependencyProperty SelectCommandProperty = DependencyProperty.Register(
        nameof(SelectCommand),
        typeof(ICommand),
        typeof(KeyButton),
        new PropertyMetadata(null));

    public KeyButton() => InitializeComponent();

    public ICommand? SelectCommand
    {
        get => (ICommand?)GetValue(SelectCommandProperty);
        set => SetValue(SelectCommandProperty, value);
    }
}
