using System.Collections.ObjectModel;

namespace Tek_Mechanical_Keyboard.UI.Pages.Keymap.Models;

/// <summary>
/// Complete editable keyboard state used by the KeyMap page.
/// </summary>
public sealed class KeyboardProfile
{
    public string Name { get; set; } = "Default";
    public int LayerCount { get; set; } = 4;
    public int ActiveLayer { get; set; }
    public ObservableCollection<KeyDefinition> Keys { get; } = new();
}
