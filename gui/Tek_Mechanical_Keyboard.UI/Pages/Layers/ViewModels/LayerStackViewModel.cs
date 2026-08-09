using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tek_Mechanical_Keyboard.Business.Layers;

namespace Tek_Mechanical_Keyboard.UI.Pages.Layers.ViewModels;

public class LayerStackViewModel(ILayersBusinessService layersService) : INotifyPropertyChanged
{
    private string _currentProfile = "Default";
    private LayerDto? _selectedLayer;

    public ObservableCollection<LayerDto> Layers { get; } = [];

    public LayerDto? SelectedLayer
    {
        get => _selectedLayer;
        set
        {
            if (SetField(ref _selectedLayer, value) && value != null)
            {
                _ = layersService.SetActiveLayerAsync(_currentProfile, value.Index);
            }
        }
    }

    public async Task LoadLayersAsync(string profileName)
    {
        _currentProfile = profileName;
        List<LayerDto> layers = await layersService.GetLayersAsync(profileName);

        Layers.Clear();
        foreach (LayerDto layer in layers)
        {
            Layers.Add(layer);
        }

        if (Layers.Count > 0)
        {
            SelectedLayer = Layers[0];
        }
    }

    public async Task AddLayerAsync(string layerName)
    {
        LayerDto newLayer = await layersService.AddLayerAsync(_currentProfile, layerName);
        Layers.Add(newLayer);
        SelectedLayer = newLayer;
    }

    public async Task RemoveLayerAsync(LayerDto layer)
    {
        if (layer.IsDefault || !Layers.Contains(layer))
        {
            return;
        }

        int index = layer.Index;
        await layersService.RemoveLayerAsync(_currentProfile, index);
        Layers.Remove(layer);

        // Re-sync UI indices
        for (int i = 0; i < Layers.Count; i++)
        {
            Layers[i].Index = i;
        }

        SelectedLayer = Layers.Count > 0 ? Layers[Math.Min(index, Layers.Count - 1)] : null;
    }

    public async Task MoveLayerAsync(int oldIndex, int newIndex)
    {
        if (oldIndex < 0 || oldIndex >= Layers.Count || newIndex < 0 || newIndex >= Layers.Count)
        {
            return;
        }

        await layersService.MoveLayerAsync(_currentProfile, oldIndex, newIndex);
        Layers.Move(oldIndex, newIndex);

        for (int i = 0; i < Layers.Count; i++)
        {
            Layers[i].Index = i;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}