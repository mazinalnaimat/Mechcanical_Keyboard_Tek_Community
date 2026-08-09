using Tek_Mechanical_Keyboard.Business.Layers;

namespace Tek_Mechanical_Keyboard.UI.Pages.Layers.Services;

public class MacroService
{
    private readonly ILayersBusinessService _layersBusinessService;

    public MacroService(ILayersBusinessService layersBusinessService)
    {
        _layersBusinessService = layersBusinessService;
    }
}