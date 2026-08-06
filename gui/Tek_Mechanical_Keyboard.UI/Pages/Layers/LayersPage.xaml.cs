using Tek_Mechanical_Keyboard.UI.Pages;
namespace Tek_Mechanical_Keyboard.UI.Pages.Layers;

using Tek_Mechanical_Keyboard.UI.Pages.Layers.ViewModels;
public partial class LayersPage : BasePage
{
    public LayersPage(KeyMapViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }
    public LayersPage()
    {
        InitializeComponent();
    }

    private void KeyboardLayout_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {

    }
}
