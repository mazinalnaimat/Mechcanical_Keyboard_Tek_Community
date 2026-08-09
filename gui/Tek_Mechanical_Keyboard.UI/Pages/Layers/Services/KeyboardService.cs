using Tek_Mechanical_Keyboard.Business.Layers;

namespace Tek_Mechanical_Keyboard.UI.Pages.Layers.Services;

public class KeyboardService
{
    private readonly ILayersBusinessService _layersBusinessService;

    public KeyboardService(ILayersBusinessService layersBusinessService)
    {
        _layersBusinessService = layersBusinessService;
    }
}