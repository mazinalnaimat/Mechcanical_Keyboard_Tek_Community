using System.Collections.Generic;

namespace Tek_Mechanical_Keyboard.Business.Layers;

public class LayerDto
{
    public int Index { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#3B82F6";
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public List<LayerKeyDefinitionDto> Keys { get; set; } = [];
}

public class LayerKeyDefinitionDto
{
    public int KeyIndex { get; set; }
    public LayerKeyActionDto? Action { get; set; }
}

public class LayerKeyActionDto
{
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Category { get; set; } = "Basic";
    public string Description { get; set; } = string.Empty;
}