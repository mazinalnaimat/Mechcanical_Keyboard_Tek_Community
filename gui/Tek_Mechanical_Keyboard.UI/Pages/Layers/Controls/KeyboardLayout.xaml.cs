using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tek_Mechanical_Keyboard.UI.Pages.Keymap.Models;

namespace Tek_Mechanical_Keyboard.UI.Pages.Layers.Controls;

public partial class KeyboardLayout : UserControl
{
    private INotifyCollectionChanged? _observableCollection;

    private readonly List<INotifyPropertyChanged> _observedKeys = new();

    // =========================================================
    // Keys
    // =========================================================

    public static readonly DependencyProperty KeysProperty =
        DependencyProperty.Register(
            nameof(Keys),
            typeof(IEnumerable<KeyDefinition>),
            typeof(KeyboardLayout),
            new PropertyMetadata(null, OnKeysChanged));

    public IEnumerable<KeyDefinition>? Keys
    {
        get => (IEnumerable<KeyDefinition>?)GetValue(KeysProperty);
        set => SetValue(KeysProperty, value);
    }

    // =========================================================
    // SelectCommand
    // =========================================================

    public static readonly DependencyProperty SelectCommandProperty =
        DependencyProperty.Register(
            nameof(SelectCommand),
            typeof(ICommand),
            typeof(KeyboardLayout),
            new PropertyMetadata(null));

    public ICommand? SelectCommand
    {
        get => (ICommand?)GetValue(SelectCommandProperty);
        set => SetValue(SelectCommandProperty, value);
    }

    // =========================================================
    // Layout Width
    // =========================================================

    private static readonly DependencyPropertyKey LayoutWidthPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(LayoutWidth),
            typeof(double),
            typeof(KeyboardLayout),
            new PropertyMetadata(0.0));

    public static readonly DependencyProperty LayoutWidthProperty =
        LayoutWidthPropertyKey.DependencyProperty;

    public double LayoutWidth
    {
        get => (double)GetValue(LayoutWidthProperty);
        private set => SetValue(LayoutWidthPropertyKey, value);
    }

    // =========================================================
    // Layout Height
    // =========================================================

    private static readonly DependencyPropertyKey LayoutHeightPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(LayoutHeight),
            typeof(double),
            typeof(KeyboardLayout),
            new PropertyMetadata(0.0));

    public static readonly DependencyProperty LayoutHeightProperty =
        LayoutHeightPropertyKey.DependencyProperty;

    public double LayoutHeight
    {
        get => (double)GetValue(LayoutHeightProperty);
        private set => SetValue(LayoutHeightPropertyKey, value);
    }

    // =========================================================
    // Constructor
    // =========================================================

    public KeyboardLayout()
    {
        InitializeComponent();
    }

    // =========================================================
    // Keys Changed
    // =========================================================

    private static void OnKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (KeyboardLayout)d;

        control.UnsubscribeFromKeys();
        control.SubscribeToKeys();

        control.UpdateLayoutSize();
    }

    // =========================================================
    // Subscribe
    // =========================================================

    private void SubscribeToKeys()
    {
        if (Keys == null)
            return;

        if (Keys is INotifyCollectionChanged collection)
        {
            _observableCollection = collection;
            _observableCollection.CollectionChanged += OnCollectionChanged;
        }

        SubscribeToKeyProperties();
    }

    private void SubscribeToKeyProperties()
    {
        if (Keys == null)
            return;

        foreach (var key in Keys)
        {
            if (key is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged += OnKeyPropertyChanged;
                _observedKeys.Add(notifyPropertyChanged);
            }
        }
    }

    // =========================================================
    // Unsubscribe
    // =========================================================

    private void UnsubscribeFromKeys()
    {
        if (_observableCollection != null)
        {
            _observableCollection.CollectionChanged -= OnCollectionChanged;
            _observableCollection = null;
        }

        foreach (var key in _observedKeys)
        {
            key.PropertyChanged -= OnKeyPropertyChanged;
        }

        _observedKeys.Clear();
    }

    // =========================================================
    // Collection Changed
    // =========================================================

    private void OnCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        // Recreate key-property subscriptions.
        foreach (var key in _observedKeys)
        {
            key.PropertyChanged -= OnKeyPropertyChanged;
        }

        _observedKeys.Clear();

        SubscribeToKeyProperties();

        UpdateLayoutSize();
    }

    // =========================================================
    // Key Changed
    // =========================================================

    private void OnKeyPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(KeyDefinition.CanvasLeft) ||
            e.PropertyName == nameof(KeyDefinition.CanvasTop) ||
            e.PropertyName == nameof(KeyDefinition.PixelWidth) ||
            e.PropertyName == nameof(KeyDefinition.PixelHeight) ||
            string.IsNullOrEmpty(e.PropertyName))
        {
            UpdateLayoutSize();
        }
    }

    // =========================================================
    // Calculate keyboard size
    // =========================================================

    private void UpdateLayoutSize()
    {
        if (Keys == null)
        {
            LayoutWidth = 0;
            LayoutHeight = 0;
            return;
        }

        var keys = Keys.ToList();

        if (keys.Count == 0)
        {
            LayoutWidth = 0;
            LayoutHeight = 0;
            return;
        }

        double maxRight = keys.Max(
            key => key.CanvasLeft + key.PixelWidth);

        double maxBottom = keys.Max(
            key => key.CanvasTop + key.PixelHeight);

        LayoutWidth = maxRight;
        LayoutHeight = maxBottom;
    }
}