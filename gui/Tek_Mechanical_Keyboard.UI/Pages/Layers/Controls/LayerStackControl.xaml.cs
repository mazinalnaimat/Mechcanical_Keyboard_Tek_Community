using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Tek_Mechanical_Keyboard.UI.Pages.Layers.Controls
{
    public partial class LayerStackControl : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(LayerStackControl),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(object),
                typeof(LayerStackControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public LayerStackControl()
        {
            InitializeComponent();
            LayersListBox.SelectionChanged += (_, _) => SelectedItem = LayersListBox.SelectedItem;
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (LayerStackControl)d;
            control.LayersListBox.ItemsSource = e.NewValue as IEnumerable;
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (LayerStackControl)d;
            if (!Equals(control.LayersListBox.SelectedItem, e.NewValue))
            {
                control.LayersListBox.SelectedItem = e.NewValue;
            }
        }

        private void LayerItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (sender is FrameworkElement element && element.DataContext is not null)
            {
                DataObject data = new DataObject();
                data.SetData("DraggedLayer", element.DataContext);
                DragDrop.DoDragDrop(element, data, DragDropEffects.Move);
            }
        }

        private void LayersListBox_Drop(object sender, DragEventArgs e)
        {
            if (ItemsSource is not IList list)
            {
                return;
            }

            object? draggedItem = e.Data.GetData("DraggedLayer");
            if (draggedItem is null)
            {
                return;
            }

            int oldIndex = list.IndexOf(draggedItem);
            if (oldIndex < 0)
            {
                return;
            }

            UIElement? originalSource = e.OriginalSource as UIElement;
            ListBoxItem? targetItem = GetParentListBoxItem(originalSource);

            int newIndex = targetItem?.DataContext is not null
                ? list.IndexOf(targetItem.DataContext)
                : list.Count - 1;

            if (newIndex < 0 || newIndex == oldIndex)
            {
                return;
            }

            if (list.GetType().GetMethod("Move") is var moveMethod && moveMethod is not null)
            {
                moveMethod.Invoke(list, new object[] { oldIndex, newIndex });
            }
            else
            {
                list.RemoveAt(oldIndex);
                list.Insert(newIndex, draggedItem);
            }

            SelectedItem = draggedItem;
        }

        private static ListBoxItem? GetParentListBoxItem(UIElement? element)
        {
            while (element is not null && element is not ListBoxItem)
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }

            return element as ListBoxItem;
        }
    }
}