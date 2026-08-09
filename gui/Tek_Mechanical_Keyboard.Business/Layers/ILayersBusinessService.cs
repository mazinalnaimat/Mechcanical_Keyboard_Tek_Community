using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tek_Mechanical_Keyboard.Business.Layers;

public interface ILayersBusinessService
{
    Task<List<LayerDto>> GetLayersAsync(string profileName, CancellationToken cancellationToken = default);
    Task<LayerDto> AddLayerAsync(string profileName, string layerName, CancellationToken cancellationToken = default);
    Task RemoveLayerAsync(string profileName, int layerIndex, CancellationToken cancellationToken = default);
    Task MoveLayerAsync(string profileName, int oldIndex, int newIndex, CancellationToken cancellationToken = default);
    Task UpdateLayerNameAsync(string profileName, int layerIndex, string newName, CancellationToken cancellationToken = default);
    Task SetActiveLayerAsync(string profileName, int layerIndex, CancellationToken cancellationToken = default);
}