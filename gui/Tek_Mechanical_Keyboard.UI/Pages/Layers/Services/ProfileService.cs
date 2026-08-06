using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tek_Mechanical_Keyboard.Business.Layers;
using Tek_Mechanical_Keyboard.UI.Pages.Layers.Models;

namespace Tek_Mechanical_Keyboard.UI.Pages.Layers.Services;

public interface IProfileService
{
    Task<IReadOnlyList<string>> GetProfileNamesAsync(CancellationToken cancellationToken = default);
    Task<KeyboardProfile> LoadProfileAsync(string profileName, CancellationToken cancellationToken = default);
    Task SaveProfileAsync(KeyboardProfile profile, CancellationToken cancellationToken = default);
}

/// <summary>
/// Profile read/write adapter. All persistence goes through the Business layer.
/// </summary>
public sealed class ProfileService : IProfileService
{
    private readonly LayersBusinessService _businessService;

    public ProfileService(LayersBusinessService businessService)
        => _businessService = businessService ?? throw new ArgumentNullException(nameof(businessService));

    public Task<IReadOnlyList<string>> GetProfileNamesAsync(CancellationToken cancellationToken = default)
        => _businessService.GetProfileNamesAsync(cancellationToken);

    public async Task<KeyboardProfile> LoadProfileAsync(string profileName, CancellationToken cancellationToken = default)
        => KeyboardService.Map(await _businessService.LoadProfileAsync(profileName, cancellationToken));

    public Task SaveProfileAsync(KeyboardProfile profile, CancellationToken cancellationToken = default)
        => _businessService.SaveProfileAsync(KeyboardService.ToDto(profile), cancellationToken);
}
