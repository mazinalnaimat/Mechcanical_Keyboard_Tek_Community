using System.Windows;
using System.Windows.Controls;

namespace Tek_Mechanical_Keyboard.UI.Left_Side_Buttons;

/// <summary>
/// Reusable navigation button shown inside the left sidebar.
/// Its click behavior and selected-state properties live in this control.
/// </summary>
public partial class LeftSideButton : UserControl
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(LeftSideButton),
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(string),
            typeof(LeftSideButton),
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty PageKeyProperty =
        DependencyProperty.Register(
            nameof(PageKey),
            typeof(string),
            typeof(LeftSideButton),
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(LeftSideButton),
            new PropertyMetadata(false));

    public static readonly RoutedEvent ClickEvent =
        EventManager.RegisterRoutedEvent(
            nameof(Click),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(LeftSideButton));

    public LeftSideButton()
    {
        InitializeComponent();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// A simple identifier used by the parent sidebar to decide which page to open.
    /// </summary>
    public string PageKey
    {
        get => (string)GetValue(PageKeyProperty);
        set => SetValue(PageKeyProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    private void ButtonPart_OnClick(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickEvent, this));
    }
}
