using System.Collections.Generic;

namespace Tek_Mechanical_Keyboard.Business.Keymap;

/// <summary>
/// Business-layer DTOs. The UI maps these DTOs to its own MVVM models so the Business
/// layer never depends on WPF controls or UI types.
/// </summary>
public sealed class KeyActionDto
{
    public string Code { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Category { get; init; } = "Basic";
    public string Description { get; init; } = string.Empty;
}

public sealed class KeyDefinitionDto
{
    public int KeyIndex { get; init; }
    public string Legend { get; init; } = string.Empty;
    public double X { get; init; }
    public double Y { get; init; }
    public double Width { get; init; } = 1.0;
    public double Height { get; init; } = 1.0;
    public bool IsModifier { get; init; }
    public KeyActionDto? Action { get; set; }
}

public sealed class KeyboardProfileDto
{
    public string Name { get; set; } = "Default";
    public int LayerCount { get; set; } = 4;
    public int ActiveLayer { get; set; }
    public List<KeyDefinitionDto> Keys { get; } = new();
}
