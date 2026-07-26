using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tek_Mechanical_Keyboard.UI.Keymap_Page;
using Tek_Mechanical_Keyboard.UI.Left_Side_Buttons;
using Tek_Mechanical_Keyboard.UI.Lighting_Page;
using Tek_Mechanical_Keyboard.UI.Macros_Page;
using Tek_Mechanical_Keyboard.UI.Main_Page;
using Tek_Mechanical_Keyboard.UI.Settings_Page;

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

        _pages = new Dictionary<string, Page>(StringComparer.OrdinalIgnoreCase)
        {
            ["Main"] = new MainPage(),
            ["Keymap"] = new KeymapPage(),
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
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleWindowState();
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ToggleWindowState()
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }
}
