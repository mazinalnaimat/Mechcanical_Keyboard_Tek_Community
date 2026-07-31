using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tek_Mechanical_Keyboard.Business.Keymap;
using Tek_Mechanical_Keyboard.UI.Left_Side_Buttons;
using Tek_Mechanical_Keyboard.UI.Pages.Keymap;
using Tek_Mechanical_Keyboard.UI.Pages.Keymap.Services;
using Tek_Mechanical_Keyboard.UI.Pages.Keymap.ViewModels;
using Tek_Mechanical_Keyboard.UI.Pages.Lighting;
using Tek_Mechanical_Keyboard.UI.Pages.Macros;
using Tek_Mechanical_Keyboard.UI.Pages.Main;
using Tek_Mechanical_Keyboard.UI.Pages.Settings;

namespace Tek_Mechanical_Keyboard.UI.Main_Window;

/// <summary>
/// Main application shell. It receives navigation requests from LeftSidePanel
/// and displays the corresponding Page on the right side.
/// </summary>
public partial class MainWindow : Window
{
    private readonly Dictionary<string, Page> _pages;

    public MainWindow()
    {
        InitializeComponent();

        // Composition root for the KeyMap feature.
        // Replace KeyMapBusinessService with the real Business implementation later;
        // KeymapPage and KeyMapViewModel do not need to change.
        IKeyMapBusinessService keyMapBusinessService = new KeyMapBusinessService();
        KeyMapViewModel keyMapViewModel = new(
            new KeyboardService(keyMapBusinessService),
            new MacroService(keyMapBusinessService),
            new ProfileService(keyMapBusinessService));

        _pages = new Dictionary<string, Page>(StringComparer.OrdinalIgnoreCase)
        {
            ["Main"] = new MainPage(),
            ["Keymap"] = new KeymapPage(keyMapViewModel),
            ["Lighting"] = new LightingPage(),
            ["Macros"] = new MacrosPage(),
            ["Settings"] = new SettingsPage()
        };

        NavigateTo("Main");
    }

    private void SidePanel_OnNavigationRequested(
        object? sender,
        NavigationRequestedEventArgs e)
    {
        NavigateTo(e.PageKey);
    }

    private void NavigateTo(string pageKey)
    {
        if (!_pages.TryGetValue(pageKey, out Page? page))
        {
            return;
        }

        PageFrame.Navigate(page);

        while (PageFrame.CanGoBack)
        {
            PageFrame.RemoveBackEntry();
        }
    }

    private void TitleBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            ToggleWindowState();
            return;
        }

        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        => WindowState = WindowState.Minimized;

    private void MaximizeButton_OnClick(object sender, RoutedEventArgs e)
        => ToggleWindowState();

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        => this.Close();


    private void ToggleWindowState()
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void SidePanel_Loaded(object sender, RoutedEventArgs e)
    {

    }
}
