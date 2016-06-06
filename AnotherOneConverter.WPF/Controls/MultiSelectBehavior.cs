using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace AnotherOneConverter.WPF.Controls {
    public class MultiSelectBehavior : Behavior<MultiSelector> {
        private static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems",
            typeof(IList), typeof(MultiSelectBehavior), new PropertyMetadata(OnSelectedItemsChanged));

        public IList SelectedItems {
            get {
                return (IList)GetValue(SelectedItemsProperty);
            }
            set {
                SetValue(SelectedItemsProperty, value);
            }
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((MultiSelectBehavior)d).OnSelectedItemsChanged(e);
        }

        private void OnSelectedItemsChanged(DependencyPropertyChangedEventArgs e) {
            if (e.OldValue is INotifyCollectionChanged) {
                ((INotifyCollectionChanged)e.OldValue).CollectionChanged -= OnSelectedItemsCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged) {
                ((INotifyCollectionChanged)e.NewValue).CollectionChanged -= OnSelectedItemsCollectionChanged;
                ((INotifyCollectionChanged)e.NewValue).CollectionChanged += OnSelectedItemsCollectionChanged;
            }
        }

        protected override void OnAttached() {
            base.OnAttached();

            AssociatedObject.SelectionChanged -= OnAssociatedObjectSelectionChanged;
            AssociatedObject.SelectionChanged += OnAssociatedObjectSelectionChanged;
        }

        protected override void OnDetaching() {
            base.OnDetaching();

            AssociatedObject.SelectionChanged -= OnAssociatedObjectSelectionChanged;
        }

        private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null) {
                foreach (var item in e.OldItems) {
                    if (AssociatedObject.SelectedItems.Contains(item)) {
                        AssociatedObject.SelectedItems.Remove(item);
                    }
                }
            }

            if (e.NewItems != null) {
                foreach (var item in e.NewItems) {
                    if (AssociatedObject.SelectedItems.Contains(item) == false) {
                        AssociatedObject.SelectedItems.Add(item);
                    }
                }
            }
        }

        private void OnAssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (SelectedItems == null)
                return;

            foreach (var item in e.RemovedItems) {
                if (SelectedItems.Contains(item)) {
                    SelectedItems.Remove(item);
                }
            }

            foreach (var item in e.AddedItems) {
                if (SelectedItems.Contains(item) == false) {
                    SelectedItems.Add(item);
                }
            }
        }
    }
}
