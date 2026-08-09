using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tek_Mechanical_Keyboard.Business.Layers;

public class LayersBusinessService : ILayersBusinessService
{
    private readonly Dictionary<string, List<LayerDto>> _profileLayers = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _lock = new(1, 1);

    public LayersBusinessService()
    {
        // Seed initial default layers
        _profileLayers["Default"] = CreateDefaultLayers();
    }

    public async Task<List<LayerDto>> GetLayersAsync(string profileName, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            List<LayerDto> layers = GetOrCreateProfileLayers(profileName);
            return layers.Select(CloneLayer).ToList();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<LayerDto> AddLayerAsync(string profileName, string layerName, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            List<LayerDto> layers = GetOrCreateProfileLayers(profileName);

            if (layers.Count >= 16)
            {
                throw new InvalidOperationException("Maximum limit of 16 layers reached.");
            }

            int newIndex = layers.Count;
            LayerDto newLayer = new()
            {
                Index = newIndex,
                Name = string.IsNullOrWhiteSpace(layerName) ? $"Layer {newIndex}" : layerName,
                IsActive = false,
                IsDefault = false,
                ColorHex = GetDefaultColorForLayer(newIndex)
            };

            layers.Add(newLayer);
            return CloneLayer(newLayer);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task RemoveLayerAsync(string profileName, int layerIndex, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            List<LayerDto> layers = GetOrCreateProfileLayers(profileName);

            if (layerIndex <= 0 || layerIndex >= layers.Count)
            {
                throw new InvalidOperationException("Cannot remove base layer or invalid layer index.");
            }

            layers.RemoveAt(layerIndex);
            ReindexLayers(layers);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task MoveLayerAsync(string profileName, int oldIndex, int newIndex, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            List<LayerDto> layers = GetOrCreateProfileLayers(profileName);

            if (oldIndex < 0 || oldIndex >= layers.Count || newIndex < 0 || newIndex >= layers.Count)
            {
                return;
            }

            LayerDto item = layers[oldIndex];
            layers.RemoveAt(oldIndex);
            layers.Insert(newIndex, item);

            ReindexLayers(layers);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task UpdateLayerNameAsync(string profileName, int layerIndex, string newName, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            List<LayerDto> layers = GetOrCreateProfileLayers(profileName);

            if (layerIndex >= 0 && layerIndex < layers.Count)
            {
                layers[layerIndex].Name = newName;
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task SetActiveLayerAsync(string profileName, int layerIndex, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            List<LayerDto> layers = GetOrCreateProfileLayers(profileName);

            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].IsActive = (i == layerIndex);
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    private List<LayerDto> GetOrCreateProfileLayers(string profileName)
    {
        if (!_profileLayers.TryGetValue(profileName, out List<LayerDto>? layers))
        {
            layers = CreateDefaultLayers();
            _profileLayers[profileName] = layers;
        }

        return layers;
    }

    private static void ReindexLayers(List<LayerDto> layers)
    {
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].Index = i;
        }
    }

    private static List<LayerDto> CreateDefaultLayers() =>
    [
        new() { Index = 0, Name = "Base", IsDefault = true, IsActive = true, ColorHex = "#3B82F6" },
        new() { Index = 1, Name = "FN 1", IsDefault = false, IsActive = false, ColorHex = "#10B981" },
        new() { Index = 2, Name = "Media", IsDefault = false, IsActive = false, ColorHex = "#F59E0B" }
    ];

    private static LayerDto CloneLayer(LayerDto layer) => new()
    {
        Index = layer.Index,
        Name = layer.Name,
        ColorHex = layer.ColorHex,
        IsActive = layer.IsActive,
        IsDefault = layer.IsDefault,
        Keys = layer.Keys.Select(k => new LayerKeyDefinitionDto
        {
            KeyIndex = k.KeyIndex,
            Action = k.Action is null ? null : new LayerKeyActionDto
            {
                Code = k.Action.Code,
                DisplayName = k.Action.DisplayName,
                Category = k.Action.Category,
                Description = k.Action.Description
            }
        }).ToList()
    };
    private static string GetDefaultColorForLayer(int index)
    {
        string[] palette = ["#3B82F6", "#10B981", "#F59E0B", "#EF4444", "#8B5CF6", "#EC4899"];
        return palette[index % palette.Length];
    }
}