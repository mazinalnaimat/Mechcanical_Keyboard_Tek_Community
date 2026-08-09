using Tek_Mechanical_Keyboard.Business.Layers;

namespace Tek_Mechanical_Keyboard.UI.Pages.Layers.Services;

public class ProfileService
{
    private readonly ILayersBusinessService _layersBusinessService;

    public ProfileService(ILayersBusinessService layersBusinessService)
    {
        _layersBusinessService = layersBusinessService;
    }
}