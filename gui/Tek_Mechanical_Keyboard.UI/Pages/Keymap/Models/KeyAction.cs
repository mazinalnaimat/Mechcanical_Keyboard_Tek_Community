namespace Tek_Mechanical_Keyboard.UI.Pages.Keymap.Models;

/// <summary>
/// One QMK action that can be assigned to a physical key.
/// The Code value is the QMK keycode/action identifier passed to the Business layer.
/// </summary>
public sealed class KeyAction
{
    public string Code { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Category { get; init; } = "Basic";
    public string Description { get; init; } = string.Empty;

    public override string ToString() => DisplayName;
}
