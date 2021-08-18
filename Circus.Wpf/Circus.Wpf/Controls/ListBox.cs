// Copyright (c) 2019-2020, Circus.
//
// Licensed under the Apache License, Version 2.0 (the "License");
//
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
//
// A list of selectable item.
//
// If ScrollViewer.HorizontalScrollBarVisibility is set to Hidden, the
// items presenter max width is constrained to the control width (minus
// the vertical scroll bar width if shown). This allows ListBoxItems to use
// their "trimming" feature if available and therefore avoid items content
// to overflow the visible area.
//
// ContentSelectionMode determines which item in the list is selected by
// default when the control is initialized or the base collection is 
// changed (remove, reset). Default value is None.
//
// GetContentSelectionOverride() allows to specify a different default 
// selection logic when overriden is a derived type. It is invoked 
// OnInitialized and OnItemsChanged.
//
// ListBox supports DataSource binding and reports ItemDoubleClicked
// event.


#pragma warning disable IDE0002

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Circus.Wpf.Data;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a list of selectable items.</summary>
    public class ListBox : System.Windows.Controls.ListBox {
        private DataSource data;
        /// <summary>Identifies the item mouse clicked routed event.</summary>
        public static readonly RoutedEvent ItemClickedEvent;
        /// <summary>Identifies the item mouse double-clicked routed event.</summary>
        public static readonly RoutedEvent ItemDoubleClickedEvent;
        /// <summary>Identifies the content selection mode dependency property.</summary>
        public static readonly DependencyProperty ContentSelectionModeProperty;
        /// <summary>Identifies the sort dependency property.</summary>
        public static readonly DependencyProperty SortProperty;
        /// <summary>Gets or sets how items are selected by default when the control is initialized or the content changed.</summary>
        [Bindable(true)]
        public ContentSelectionMode ContentSelectionMode { get => (ContentSelectionMode)this.GetValue(ListBox.ContentSelectionModeProperty); set => this.SetValue(ListBox.ContentSelectionModeProperty, value); }
        /// <summary>Gets or sets the SortDescription used to sort items.</summary>
        [Bindable(true)]
        public SortDescription Sort { get => (SortDescription)this.GetValue(ListBox.SortProperty); set => this.SetValue(ListBox.SortProperty, value); }
        static ListBox() {
            ListBox.ContentSelectionModeProperty = DependencyProperty.Register("ContentSelectionMode", typeof(ContentSelectionMode), typeof(ListBox), new FrameworkPropertyMetadata(ContentSelectionMode.None));
            ListBox.DataContextProperty.OverrideMetadata(typeof(ListBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ListBox.OnDataContextChanged)));
            ListBox.ItemClickedEvent = EventManager.RegisterRoutedEvent("ItemClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ListBox));
            ListBox.ItemDoubleClickedEvent = EventManager.RegisterRoutedEvent("ItemDoubleClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ListBox));
            ListBox.SortProperty = DependencyProperty.Register("Sort", typeof(SortDescription), typeof(ListBox), new FrameworkPropertyMetadata(new SortDescription(), new PropertyChangedCallback(ListBox.OnSortChanged)));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ListBox), new FrameworkPropertyMetadata(typeof(ListBox)));
        }
        public ListBox() {
            this.Items.IsLiveSorting = true;
        }
        private void AddSort(SortDescription description) {
            this.Items.SortDescriptions.Add(description);
            this.Items.LiveSortingProperties.Add(description.PropertyName);
        }
        protected override DependencyObject GetContainerForItemOverride() {
            return new ListBoxItem();
        }
        /// <summary>Determines which item is selected by default when the control is initialized or the content changed.</summary>
        protected virtual int GetContentSelectionOverride() {
            int num = base.Items.Count;
            return num > 0 ? this.ContentSelectionMode == ContentSelectionMode.First ? 0 : num - 1 : -1;
        }
        protected override bool IsItemItsOwnContainerOverride(object item) {
            return Assert.Is<ListBoxItem>(item);
        }
        internal void NotifyItemClick(ListBoxItem item) {
            this.OnItemClicked(new RoutedEventArgs(ListBox.ItemClickedEvent, item));
        }
        internal void NotifyItemDoubleClick(ListBoxItem item) {
            if (Assert.NotNull(data) && data.DoubleClickVisible) {
                data.Invoke(data.OnItemDoubleClicked, this, new DataEventArgs(item));
            }
            this.OnItemDoubleClicked(new RoutedEventArgs(ListBox.ItemDoubleClickedEvent, item));
        }
        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((ListBox)d).data = Assert.Is(e.NewValue, out DataSource value) ? value : null;
        }
        protected override void OnInitialized(EventArgs e) {
            base.OnInitialized(e);
            this.SetSelection();
        }
        /// <summary>Raises the ListBox.ItemClicked event. This method is invoked when a list box item is mouse clicked.</summary>
        protected virtual void OnItemClicked(RoutedEventArgs e) {
            this.RaiseEvent(e);
        }
        /// <summary>Raises the ListBox.ItemDoubleClicked event. This method is invoked when a list box item is mouse double-clicked.</summary>
        protected virtual void OnItemDoubleClicked(RoutedEventArgs e) {
            this.RaiseEvent(e);
        }
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
            base.OnItemsChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset) {
                this.SetSelection();
            }
        }
        protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
            if (Assert.NotNull(this.data) && e.AddedItems.Count > 0) {
                data.Invoke(data.OnSelectedItemChanged, this, new DataEventArgs(e.AddedItems[0]));
            }
            base.OnSelectionChanged(e);
        }
        private void OnSortChanged(object value) {
            if (this.Items.SortDescriptions.Count == 1) {
                this.Items.SortDescriptions.RemoveAt(0);
            }
            if (Assert.NotNull(value)) {
                this.AddSort((SortDescription)value);
            }
        }
        private static void OnSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((ListBox)d).OnSortChanged(e.NewValue);
        }
        private void SetSelection() {
            if (this.ContentSelectionMode != ContentSelectionMode.None && base.SelectedIndex == -1) {
                base.SelectedIndex = this.GetContentSelectionOverride();
            }
        }
    }
}
