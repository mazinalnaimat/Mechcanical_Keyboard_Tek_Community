using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tek_Mechanical_Keyboard.Business.Keymap;
using Tek_Mechanical_Keyboard.UI.Pages.Keymap.Models;

namespace Tek_Mechanical_Keyboard.UI.Pages.Keymap.Services;

public interface IMacroService
{
    Task<IReadOnlyList<KeyAction>> GetAvailableActionsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Loads assignable QMK actions (including macro actions) from the Business layer.
/// </summary>
public sealed class MacroService : IMacroService
{
    private readonly IKeyMapBusinessService _businessService;

    public MacroService(IKeyMapBusinessService businessService)
        => _businessService = businessService ?? throw new ArgumentNullException(nameof(businessService));

    public async Task<IReadOnlyList<KeyAction>> GetAvailableActionsAsync(CancellationToken cancellationToken = default)
        => (await _businessService.GetAvailableActionsAsync(cancellationToken))
            .Select(KeyboardService.Map)
            .ToArray();
}
