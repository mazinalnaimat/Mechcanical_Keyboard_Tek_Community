using System.Windows.Controls;

namespace Tek_Mechanical_Keyboard.UI.Pages;

public  class BasePage : Page
{
    public const double DefaultPageMinWidth = 900;
    public const double DefaultPageMinHeight = 650;

    public BasePage()
    {

        MinWidth = DefaultPageMinWidth;
        MinHeight = DefaultPageMinHeight;

        SetResourceReference(BackgroundProperty, "PageBackgroundBrush");

        UseLayoutRounding = true;
        SnapsToDevicePixels = true;
    }
}