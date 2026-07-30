using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Tek_Mechanical_Keyboard.UI.Common.Commands;
using Tek_Mechanical_Keyboard.UI.Pages.Keymap.Models;
using Tek_Mechanical_Keyboard.UI.Pages.Keymap.Services;

namespace Tek_Mechanical_Keyboard.UI.Pages.Keymap.ViewModels;

/// <summary>
/// MVVM presentation logic for KeymapPage.
/// This ViewModel only talks to feature services; services are the boundary to Business.
/// </summary>
public sealed class KeyMapViewModel : INotifyPropertyChanged
{
    private readonly IKeyboardService _keyboardService;
    private readonly IMacroService _macroService;
    private readonly IProfileService _profileService;

    private KeyboardProfile? _profile;
    private KeyDefinition? _selectedKey;
    private KeyAction? _selectedAction;
    private string? _selectedProfileName;
    private string _selectedCategory = "Basic";
    private string _searchText = string.Empty;
    private int _selectedLayer;
    private bool _isBusy;
    private string _statusMessage = "Ready";

    public KeyMapViewModel(
        IKeyboardService keyboardService,
        IMacroService macroService,
        IProfileService profileService)
    {
        _keyboardService = keyboardService ?? throw new ArgumentNullException(nameof(keyboardService));
        _macroService = macroService ?? throw new ArgumentNullException(nameof(macroService));
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));

        LoadCommand = new RelayCommand(InitializeAsync, () => !IsBusy);
        RefreshCommand = new RelayCommand(RefreshKeyboardAsync, () => !IsBusy);
        SaveProfileCommand = new RelayCommand(SaveProfileAsync, () => Profile is not null && !IsBusy);
        LoadProfileCommand = new RelayCommand(LoadSelectedProfileAsync, () => !string.IsNullOrWhiteSpace(SelectedProfileName) && !IsBusy);
        SelectKeyCommand = new RelayCommand(SelectKey);
        SelectActionCommand = new RelayCommand(SelectAction);
        ApplyActionCommand = new RelayCommand(ApplySelectedActionAsync, () => SelectedKey is not null && SelectedAction is not null && !IsBusy);
        ResetKeyCommand = new RelayCommand(ResetSelectedKeyAsync, () => SelectedKey is not null && !IsBusy);

        _ = InitializeAsync();
    }

    public ObservableCollection<KeyDefinition> Keys { get; } = new();
    public ObservableCollection<KeyAction> AvailableActions { get; } = new();
    public ObservableCollection<KeyAction> FilteredActions { get; } = new();
    public ObservableCollection<string> ProfileNames { get; } = new();
    public ObservableCollection<string> ActionCategories { get; } = new() { "Basic", "Media", "Macro", "Layer", "Lighting", "System" };
    public ObservableCollection<int> Layers { get; } = new();

    public KeyboardProfile? Profile
    {
        get => _profile;
        private set
        {
            if (ReferenceEquals(_profile, value))
            {
                return;
            }

            _profile = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ProfileTitle));
            RaiseCommandStates();
        }
    }

    public string ProfileTitle => Profile is null ? "Keyboard" : $"{Profile.Name} · {Keys.Count} keys";

    public KeyDefinition? SelectedKey
    {
        get => _selectedKey;
        private set
        {
            if (ReferenceEquals(_selectedKey, value))
            {
                return;
            }

            if (_selectedKey is not null)
            {
                _selectedKey.IsSelected = false;
            }

            _selectedKey = value;

            if (_selectedKey is not null)
            {
                _selectedKey.IsSelected = true;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedKeyTitle));
            RaiseCommandStates();
        }
    }

    public string SelectedKeyTitle => SelectedKey is null ? "No key selected" : $"Key {SelectedKey.KeyIndex}: {SelectedKey.Legend}";

    public KeyAction? SelectedAction
    {
        get => _selectedAction;
        set
        {
            if (ReferenceEquals(_selectedAction, value))
            {
                return;
            }

            _selectedAction = value;
            OnPropertyChanged();
            RaiseCommandStates();
        }
    }

    public string? SelectedProfileName
    {
        get => _selectedProfileName;
        set
        {
            if (_selectedProfileName == value)
            {
                return;
            }

            _selectedProfileName = value;
            OnPropertyChanged();
            RaiseCommandStates();
        }
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (_selectedCategory == value)
            {
                return;
            }

            _selectedCategory = value;
            OnPropertyChanged();
            RefreshActionFilter();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value)
            {
                return;
            }

            _searchText = value;
            OnPropertyChanged();
            RefreshActionFilter();
        }
    }

    public int SelectedLayer
    {
        get => _selectedLayer;
        set
        {
            if (_selectedLayer == value)
            {
                return;
            }

            _selectedLayer = value;
            if (Profile is not null)
            {
                Profile.ActiveLayer = value;
            }

            OnPropertyChanged();
            StatusMessage = $"Layer {value} selected";
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value)
            {
                return;
            }

            _isBusy = value;
            OnPropertyChanged();
            RaiseCommandStates();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (_statusMessage == value)
            {
                return;
            }

            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public ICommand LoadCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SaveProfileCommand { get; }
    public ICommand LoadProfileCommand { get; }
    public ICommand SelectKeyCommand { get; }
    public ICommand SelectActionCommand { get; }
    public ICommand ApplyActionCommand { get; }
    public ICommand ResetKeyCommand { get; }

    /// <summary>Gets keyboard layout, actions and profile names through the Business-connected services.</summary>
    public async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            KeyboardProfile profile = await _keyboardService.GetKeyboardAsync();
            IReadOnlyList<KeyAction> actions = await _macroService.GetAvailableActionsAsync();
            IReadOnlyList<string> profileNames = await _profileService.GetProfileNamesAsync();

            SetProfile(profile);
            Replace(AvailableActions, actions);
            Replace(ProfileNames, profileNames);
            SelectedProfileName = ProfileNames.FirstOrDefault() ?? profile.Name;
            RefreshActionFilter();
            StatusMessage = "Keyboard data loaded from Business layer";
        });
    }

    /// <summary>Reads the latest keyboard data from the Business layer.</summary>
    public async Task RefreshKeyboardAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            SetProfile(await _keyboardService.GetKeyboardAsync());
            StatusMessage = "Keyboard refreshed";
        });
    }

    /// <summary>Loads the selected profile through the Business layer.</summary>
    public async Task LoadSelectedProfileAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedProfileName))
        {
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            SetProfile(await _profileService.LoadProfileAsync(SelectedProfileName));
            StatusMessage = $"Profile '{SelectedProfileName}' loaded";
        });
    }

    /// <summary>Sends the complete edited profile to the Business layer.</summary>
    public async Task SaveProfileAsync()
    {
        if (Profile is null)
        {
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            await _profileService.SaveProfileAsync(Profile);
            StatusMessage = $"Profile '{Profile.Name}' saved";
        });
    }

    /// <summary>Sends one key/action change to the Business layer.</summary>
    public async Task ApplySelectedActionAsync()
    {
        if (Profile is null || SelectedKey is null || SelectedAction is null)
        {
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            await _keyboardService.SetKeyActionAsync(Profile.Name, SelectedLayer, SelectedKey, SelectedAction);
            SelectedKey.Action = SelectedAction;
            StatusMessage = $"{SelectedAction.DisplayName} assigned to {SelectedKey.Legend}";
        });
    }

    /// <summary>Requests that Business resets the selected key on the active layer.</summary>
    public async Task ResetSelectedKeyAsync()
    {
        if (Profile is null || SelectedKey is null)
        {
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            await _keyboardService.ResetKeyAsync(Profile.Name, SelectedLayer, SelectedKey);
            SelectedKey.Action = null;
            StatusMessage = $"{SelectedKey.Legend} reset";
        });
    }

    private void SelectKey(object? parameter)
    {
        if (parameter is KeyDefinition key)
        {
            SelectedKey = key;
            SelectedAction = key.Action;
        }
    }

    private void SelectAction(object? parameter)
    {
        if (parameter is KeyAction action)
        {
            SelectedAction = action;
        }
    }

    private void SetProfile(KeyboardProfile profile)
    {
        Profile = profile;
        Replace(Keys, profile.Keys);
        Replace(Layers, Enumerable.Range(0, Math.Max(1, profile.LayerCount)));
        SelectedLayer = Math.Clamp(profile.ActiveLayer, 0, Math.Max(0, profile.LayerCount - 1));
        SelectedKey = null;
        OnPropertyChanged(nameof(ProfileTitle));
    }

    private void RefreshActionFilter()
    {
        IEnumerable<KeyAction> query = AvailableActions.Where(a =>
            string.Equals(a.Category, SelectedCategory, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(a =>
                a.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                a.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                a.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        Replace(FilteredActions, query);
    }

    private async Task ExecuteBusyAsync(Func<Task> action)
    {
        try
        {
            IsBusy = true;
            await action();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static void Replace<T>(ObservableCollection<T> target, IEnumerable<T> source)
    {
        target.Clear();
        foreach (T item in source)
        {
            target.Add(item);
        }
    }

    private void RaiseCommandStates()
    {
        foreach (ICommand command in new[]
                 {
                     LoadCommand, RefreshCommand, SaveProfileCommand, LoadProfileCommand,
                     ApplyActionCommand, ResetKeyCommand
                 })
        {
            if (command is RelayCommand relay)
            {
                relay.RaiseCanExecuteChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
