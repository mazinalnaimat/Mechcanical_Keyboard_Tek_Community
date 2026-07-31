using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tek_Mechanical_Keyboard.UI.Pages.Keymap.Models;

/// <summary>
/// UI model for one physical keyboard key. Position and size are stored in keyboard units,
/// allowing the Business layer to load real layout information from QMK/MCU data later.
/// </summary>
public sealed class KeyDefinition : INotifyPropertyChanged
{
    private const double UnitPixels = 50.0;
    private const double GapPixels = 5.0;

    private KeyAction? _action;
    private bool _isSelected;

    public int KeyIndex { get; init; }
    public string Legend { get; init; } = string.Empty;
    public double X { get; init; }
    public double Y { get; init; }
    public double Width { get; init; } = 1.0;
    public double Height { get; init; } = 1.0;
    public bool IsModifier { get; init; }

    public double CanvasLeft => X * (UnitPixels + GapPixels);
    public double CanvasTop => Y * (UnitPixels + GapPixels);
    public double PixelWidth => (Width * UnitPixels) + ((Width - 1) * GapPixels);
    public double PixelHeight => (Height * UnitPixels) + ((Height - 1) * GapPixels);

    public KeyAction? Action
    {
        get => _action;
        set
        {
            if (ReferenceEquals(_action, value) ||
                (_action?.Code == value?.Code && _action?.DisplayName == value?.DisplayName))
            {
                return;
            }

            _action = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ActionDisplayName));
        }
    }

    public string ActionDisplayName => Action?.DisplayName ?? Legend;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }

            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
