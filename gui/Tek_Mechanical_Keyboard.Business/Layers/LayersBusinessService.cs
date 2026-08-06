using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tek_Mechanical_Keyboard.Business.Layers;

/// <summary>
/// Business boundary used by the WPF Layers feature.
/// Replace the current preview implementation with the real QMK/DataAccess implementation
/// without changing KeyMapPage or KeyMapViewModel.
/// </summary>
public interface LayersBusinessService
{
    Task<KeyboardProfileDto> GetKeyboardAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KeyActionDto>> GetAvailableActionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetProfileNamesAsync(CancellationToken cancellationToken = default);
    Task<KeyboardProfileDto> LoadProfileAsync(string profileName, CancellationToken cancellationToken = default);
    Task SaveProfileAsync(KeyboardProfileDto profile, CancellationToken cancellationToken = default);
    Task SetKeyActionAsync(string profileName, int layer, int keyIndex, KeyActionDto action, CancellationToken cancellationToken = default);
    Task ResetKeyAsync(string profileName, int layer, int keyIndex, CancellationToken cancellationToken = default);
}
