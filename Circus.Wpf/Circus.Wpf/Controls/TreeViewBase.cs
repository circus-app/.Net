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
// A base class for a treeview control.
//
// The control is specifically designed to be used with a data source binding and
// data templating to preserve flexibility of implementation and provide standard
// features across derived types.
//
// The main functionality is filtering. Because it can be achieved in several ways
// (see TreeView for details), it focuses on layout considerations only, the
// filtering logic being implemented in derived types.
//
// On filtered, the treeview expands all resulting nodes and when the filter is
// removed, it restores the view as it was prior to filtering. This is done by
// caching each node state (expanded or collapsed) and the vertical scroll
// position to an ITreeViewSource object.
//
// Because treeview items are virtualized, the control is notified of changes
// using the OnItemAdded/OnItemToggled methods to maintain an updated cache.
//
// If a filter operation returns an empty result, the control shows a "No results 
// found." message block that is defined in template.
//
// The control provides Expand/Collapse routed commands.
//
// As previously mentioned, it is intended to be bound to a DataSource object and
// therefore, it notifies of the following events: selected item changed, item
// double-click and drag and drop completed.
//
// TreeView does not support sorting. If required, sort must be applied on the
// source collection prior to binding.


#pragma warning disable IDE0002

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Circus.Wpf.Data;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a base class for a treeview control.</summary>
    [ClassCommand("Collapse")]
    [ClassCommand("Expand")]
    [TemplatePart(Name = "Scroll", Type = typeof(ScrollViewer))]
    public abstract class TreeViewBase : System.Windows.Controls.TreeView, IExpandable, IFilterable {
        private object source;
        /// <summary>Identifies the is filtered property.</summary>
        public static readonly DependencyProperty IsFilteredProperty;
        /// <summary>Identifies the is filtered result empty property.</summary>
        public static readonly DependencyProperty IsFilterResultEmptyProperty;
        /// <summary>Identifies the state property.</summary>
        public static readonly DependencyProperty StateProperty;
        private double ContentVerticalOffset { get; set; }
        protected DataSource DataSource { get; set; }
        [Bindable(true)]
        public bool IsFiltered { get => (bool)this.GetValue(TreeViewBase.IsFilteredProperty); private set => this.SetValue(TreeViewBase.IsFilteredProperty, value); }
        [Bindable(true)]
        public bool IsFilterResultEmpty { get => (bool)this.GetValue(TreeViewBase.IsFilterResultEmptyProperty); private set => this.SetValue(TreeViewBase.IsFilterResultEmptyProperty, value); }
        /// <summary>Gets or sets the scroll viewer control defined and named accordingly in the template.</summary>
        protected ScrollViewer Scroll { get; set; }
        /// <summary>Gets or sets the ITreeViewSource object used for caching.</summary>
        protected ITreeViewSource Source { get; set; }
        [Bindable(true)]
        public ExpandableState State { get => (ExpandableState)this.GetValue(TreeViewBase.StateProperty); set => this.SetValue(TreeViewBase.StateProperty, value); }
        static TreeViewBase() {
            TreeViewBase.DataContextProperty.OverrideMetadata(typeof(TreeViewBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(TreeViewBase.OnDataContextChanged)));
            TreeViewBase.IsFilteredProperty = DependencyProperty.Register("IsFiltered", typeof(bool), typeof(TreeViewBase), new FrameworkPropertyMetadata(false));
            TreeViewBase.IsFilterResultEmptyProperty = DependencyProperty.Register("IsFilterResultEmpty", typeof(bool), typeof(TreeViewBase), new FrameworkPropertyMetadata(false));
            TreeViewBase.StateProperty = DependencyProperty.Register("State", typeof(ExpandableState), typeof(TreeViewBase), new FrameworkPropertyMetadata(ExpandableState.Indeterminate, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(TreeView.OnStateChanged)));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewBase), new FrameworkPropertyMetadata(typeof(TreeViewBase)));
        }
        /// <summary>Constructs a TreeViewBase.</summary>
        protected TreeViewBase() {
            this.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(this.OnLoaded));
        }
        private static void Collapse(object sender, ExecutedRoutedEventArgs e) {
            ((TreeViewBase)sender).State = ExpandableState.Collapsed;
        }
        private static void Expand(object sender, ExecutedRoutedEventArgs e) {
            ((TreeViewBase)sender).State = ExpandableState.Expanded;
        }
        /// <summary>Filters the treeview using the specified empty result flag.</summary>
        public virtual void Filter(bool empty) {
            if (!this.IsFiltered) {
                if (this.Scroll != null) {
                    this.ContentVerticalOffset = this.Scroll.ContentVerticalOffset;
                }
                this.IsFiltered = true;
            }
            this.IsFilterResultEmpty = empty;
        }
        protected override DependencyObject GetContainerForItemOverride() {
            return new TreeViewItem();
        }
        protected override bool IsItemItsOwnContainerOverride(object item) {
            return item is TreeViewItem;
        }
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            TemplateItems.Register(this);
        }
        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((TreeViewBase)d).DataSource = Assert.Is(e.NewValue, out DataSource data) ? data : null;
        }
        /// <summary>Notifies the data source that the drag operation completed providing the specified target object.</summary>
        public virtual void OnDragCompleted(object target) {
            if (target != this.source && this.DataSource != null) {
                this.DataSource.Invoke(this.DataSource.OnDragCompleted, this, new Data.DragEventArgs(this.source, target));
            }
            this.source = null;
        }
        /// <summary>Raised when a drag operation started.</summary>
        public virtual void OnDragStarted(object source) {
            this.source = source;
        }
        /// <summary>Raised when a new TreeViewItem is added.</summary>
        public virtual void OnItemAdded(TreeViewItem item) {
            if (this.DataSource != null) {
                PropertyChangedManager.AddHandler(this.DataSource, item.DataContext);
            }
            this.Source.Add(item);
        }
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
            base.OnItemsChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (object o in e.OldItems) {
                    if (this.DataSource != null) {
                        PropertyChangedManager.RemoveHandler(this.DataSource, o);
                    }
                    this.Source.Remove(o.GetHashCode());
                }
            }
        }
        /// <summary>Notifies the data source that the specified item has been double-clicked.</summary>
        public virtual void OnItemDoubleClick(TreeViewItem item) {
            if (this.DataSource != null) {
                this.DataSource.Invoke(this.DataSource.OnItemDoubleClicked, item, new DataEventArgs(item.DataContext));
            }
        }
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
            base.OnItemsSourceChanged(oldValue, newValue);
            if (this.Source != null) {
                this.Source.Dispose();
            }
        }
        /// <summary>Raised when a TreeViewItem has been toggled.</summary>
        public virtual void OnItemToggled(TreeViewItem item) {
            this.State = ExpandableState.Indeterminate;
            if (!this.IsFiltered) {
                this.Source.Toggle(item);
            }
        }
        private void OnLoaded(object sender, RoutedEventArgs e) {
            if (this.State == ExpandableState.Expanded) {
                this.Toggle(true);
            }
        }
        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e) {
            if (this.DataSource != null) {
                this.DataSource.Invoke(this.DataSource.OnSelectedItemChanged, this, new DataEventArgs(e.NewValue));
            }
            base.OnSelectedItemChanged(e);
        }
        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((TreeViewBase)d).OnStateChanged((ExpandableState)e.NewValue);
        }
        private void OnStateChanged(ExpandableState state) {
            if (state > 0 && this.IsLoaded) {
                this.Toggle(state != ExpandableState.Collapsed);
            }
        }
        /// <summary>Restores the treeview to its original state.</summary>
        public virtual void Restore() {
            if (this.Scroll != null) {
                this.Scroll.ScrollToVerticalOffset(this.ContentVerticalOffset);
            }
            this.State = ExpandableState.Indeterminate;
            this.IsFiltered = false;
            this.IsFilterResultEmpty = false;
        }
        private void Toggle(bool value) {
            for (int i = 0; i < this.Items.Count; i++) {
                this.Toggle((TreeViewItem)this.ItemContainerGenerator.ContainerFromIndex(i), value);
            }
        }
        private void Toggle(TreeViewItem item, bool value) {
            if (value) {
                item.ExpandSubtree();
                return;
            }
            item.CollapseSubtree();
        }
    }
}
