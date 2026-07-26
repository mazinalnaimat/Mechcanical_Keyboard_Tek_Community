namespace Tek_Mechanical_Keyboard.UI.Left_Side_Buttons;

/// <summary>
/// Contains the page identifier requested by the left-side panel.
/// </summary>
public sealed class NavigationRequestedEventArgs : EventArgs
{
    public NavigationRequestedEventArgs(string pageKey)
    {
        PageKey = pageKey;
    }

    public string PageKey { get; }
}
