using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Tek_Mechanical_Keyboard.UI.Left_Side_Buttons;

/// <summary>
/// Hosts all left-side navigation buttons and exposes the selected page through
/// SelectedPageKey and the NavigationRequested event.
/// </summary>
public partial class LeftSidePanel : UserControl
{
    private readonly List<LeftSideButton> _buttons;

    public static readonly DependencyProperty IsCompactProperty =
        DependencyProperty.Register(
            nameof(IsCompact),
            typeof(bool),
            typeof(LeftSidePanel),
            new PropertyMetadata(false));

    public LeftSidePanel()
    {
        InitializeComponent();

        _buttons = new List<LeftSideButton>
        {
            MainButton,
            KeymapButton,
            LightingButton,
            MacrosButton,
            SettingsButton
        };

        SelectButton(MainButton, raiseNavigationEvent: false);
    }

    /// <summary>
    /// Raised after one of the sidebar buttons is pressed.
    /// MainWindow subscribes to this event and displays the matching page.
    /// </summary>
    public event EventHandler<NavigationRequestedEventArgs>? NavigationRequested;

    /// <summary>
    /// Raised after the sidebar toggle button is clicked and IsCompact has changed.
    /// The parent control/window can subscribe to this event.
    /// </summary>
    public event EventHandler? ToggleClicked;

    public string SelectedPageKey { get; private set; } = "Main";

    public bool IsCompact
    {
        get => (bool)GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    public bool IsMainPressed => MainButton.IsSelected;
    public bool IsKeymapPressed => KeymapButton.IsSelected;
    public bool IsLightingPressed => LightingButton.IsSelected;
    public bool IsMacrosPressed => MacrosButton.IsSelected;
    public bool IsSettingsPressed => SettingsButton.IsSelected;

    private void SidebarToggleButton_OnClick(object sender, RoutedEventArgs e)
    {
        IsCompact = !IsCompact;
        ToggleClicked?.Invoke(this, EventArgs.Empty);
    }

    private void NavigationButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is LeftSideButton button)
        {
            SelectButton(button, raiseNavigationEvent: true);
        }
    }

    private void SelectButton(LeftSideButton selectedButton, bool raiseNavigationEvent)
    {
        foreach (LeftSideButton button in _buttons)
        {
            button.IsSelected = ReferenceEquals(button, selectedButton);
        }

        SelectedPageKey = selectedButton.PageKey;

        if (raiseNavigationEvent)
        {
            NavigationRequested?.Invoke(
                this,
                new NavigationRequestedEventArgs(SelectedPageKey));
        }
    }
}
