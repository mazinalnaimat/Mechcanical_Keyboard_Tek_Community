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

    public string SelectedPageKey { get; private set; } = "Main";

    public bool IsMainPressed => MainButton.IsSelected;
    public bool IsKeymapPressed => KeymapButton.IsSelected;
    public bool IsLightingPressed => LightingButton.IsSelected;
    public bool IsMacrosPressed => MacrosButton.IsSelected;
    public bool IsSettingsPressed => SettingsButton.IsSelected;

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
