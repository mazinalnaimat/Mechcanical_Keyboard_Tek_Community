using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tek_Mechanical_Keyboard.Business.Keymap;
using Tek_Mechanical_Keyboard.UI.Pages.Keymap.Models;

namespace Tek_Mechanical_Keyboard.UI.Pages.Keymap.Services;

public interface IKeyboardService
{
    Task<KeyboardProfile> GetKeyboardAsync(CancellationToken cancellationToken = default);
    Task SetKeyActionAsync(string profileName, int layer, KeyDefinition key, KeyAction action, CancellationToken cancellationToken = default);
    Task ResetKeyAsync(string profileName, int layer, KeyDefinition key, CancellationToken cancellationToken = default);
}

/// <summary>
/// UI service adapter for keyboard read/write operations.
/// It talks to the Business layer and maps Business DTOs to UI MVVM models.
/// </summary>
public sealed class KeyboardService : IKeyboardService
{
    private readonly IKeyMapBusinessService _businessService;

    public KeyboardService(IKeyMapBusinessService businessService)
        => _businessService = businessService ?? throw new ArgumentNullException(nameof(businessService));

    public async Task<KeyboardProfile> GetKeyboardAsync(CancellationToken cancellationToken = default)
        => Map(await _businessService.GetKeyboardAsync(cancellationToken));

    public Task SetKeyActionAsync(
        string profileName,
        int layer,
        KeyDefinition key,
        KeyAction action,
        CancellationToken cancellationToken = default)
        => _businessService.SetKeyActionAsync(
            profileName,
            layer,
            key.KeyIndex,
            ToDto(action),
            cancellationToken);

    public Task ResetKeyAsync(
        string profileName,
        int layer,
        KeyDefinition key,
        CancellationToken cancellationToken = default)
        => _businessService.ResetKeyAsync(profileName, layer, key.KeyIndex, cancellationToken);

    internal static KeyboardProfile Map(KeyboardProfileDto source)
    {
        KeyboardProfile profile = new()
        {
            Name = source.Name,
            LayerCount = source.LayerCount,
            ActiveLayer = source.ActiveLayer
        };

        foreach (KeyDefinitionDto key in source.Keys)
        {
            profile.Keys.Add(new KeyDefinition
            {
                KeyIndex = key.KeyIndex,
                Legend = key.Legend,
                X = key.X,
                Y = key.Y,
                Width = key.Width,
                Height = key.Height,
                IsModifier = key.IsModifier,
                Action = key.Action is null ? null : Map(key.Action)
            });
        }

        return profile;
    }

    internal static KeyAction Map(KeyActionDto source)
        => new()
        {
            Code = source.Code,
            DisplayName = source.DisplayName,
            Category = source.Category,
            Description = source.Description
        };

    internal static KeyboardProfileDto ToDto(KeyboardProfile source)
    {
        KeyboardProfileDto profile = new()
        {
            Name = source.Name,
            LayerCount = source.LayerCount,
            ActiveLayer = source.ActiveLayer
        };

        foreach (KeyDefinition key in source.Keys)
        {
            profile.Keys.Add(new KeyDefinitionDto
            {
                KeyIndex = key.KeyIndex,
                Legend = key.Legend,
                X = key.X,
                Y = key.Y,
                Width = key.Width,
                Height = key.Height,
                IsModifier = key.IsModifier,
                Action = key.Action is null ? null : ToDto(key.Action)
            });
        }

        return profile;
    }

    internal static KeyActionDto ToDto(KeyAction source)
        => new()
        {
            Code = source.Code,
            DisplayName = source.DisplayName,
            Category = source.Category,
            Description = source.Description
        };
}
