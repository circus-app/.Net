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
// A selectable item in a TreeView control.
//
// This is mainly a styled component more than a featured control. The idea was
// to separate appearance from functionalities as treeviews within the package 
// are intended to be data templated to provide a maximum of flexibility in
// implementations.
//
// The control automatically binds IsEnabled and IsExpanded to its DataContext 
// object if it is declared like in the following example:
//
// [IsDoubleClickVisible(true)]
// public class Foo : ObservableObject {
//      public bool IsEnabled {
//          get => (bool)base.GetValue(false);
//          set => base.SetValue(value); 
//      }
//      public bool IsExpanded { 
//          get => (bool)base.GetValue(false);
//          set => base.SetValue(value); 
//      }
//      public Foo() {
//          this.IsEnabled = false;
//          this.IsExpanded = true;
//      }
//      ...
// }
//
// IsDoubleClickVisible determines if the TreeViewItem notifies its data source
// when double clicking occurs. The default value is false. Because TreeViewItem
// is generic and hierarchically data templated, it allows to detemine which 
// item is relevant for event handling (i.e. an item that is a folder containing 
// sub items should probably not report the event, whereas an item that is a 
// document should trigger a document open action, etc.).
//
// The value is based on the DataContext object IsDoubleClickVisibleAttribute if
// defined and is updated whenever DataContext changes.
//
// Indent defines the left margin applied to item relative to its parent.
//
// The control supports full-row selection by calculating its left margin relative
// to its root at initialization and storing the result in the SelectionMargin
// property. This allows to expand the selection to the parent treeview left 
// border.
//
// IsSelected is set to true on mouse left button click to allow selection changes
// on context menu open.
//
// It contains a Rename routed command that searches for a descendant child of
// type IEditable. If found, it invokes the Edit method.
//
// On load, the control hooks its context menu property to any visual child which
// context menu is not null. This allows to show the context menu using the mouse
// right click anywhere within the control bounds. Moreover, it allows to define
// different context menus for each data template. 
// 
// Because packaged treeviews are filterable, they need to maintain a cache of
// each node expansion to switch from filtered view/normal view (see TreeViewBase
// for details). The control notifies its parent of state changes on expanded or
// collapsed events.
//
// OnKeyDown override disables base event to prevent add/substract key commands 
// being executed (expand/collapse self or parent). This is to avoid renamable
// item to lose focus if the text contains a plus/minus sign (using the numeric 
// pad of the keyboard).


#pragma warning disable IDE0002

using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Circus.Wpf.Data;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a selectable item in a TreeView control.</summary>
    [ClassCommand("Rename", ModifierKeys.None, Key.F2)]
    public class TreeViewItem : System.Windows.Controls.TreeViewItem {
        private DragInfo info;
        private Size size;
        /// <summary>Identifies the is dragged over property.</summary>
        public static readonly DependencyProperty IsDraggedOverProperty;
        /// <summary>Identifies the is dragging property.</summary>
        public static readonly DependencyProperty IsDraggingProperty;
        /// <summary>Identifies the indent property.</summary>
        public static readonly DependencyProperty IndentProperty;
        /// <summary>Identifies the selection margin property.</summary>
        public static readonly DependencyProperty SelectionMarginProperty;
        /// <summary>Returns the coordinates and size in screen coordinates of the item.</summary>
        public Rect Bounds {
            get {
                return Screen.GetBounds(this, this.size, false);
            }
        }
        /// <summary>Gets or sets the left margin applied to the item relative to its parent item.</summary>
        [Bindable(true)]
        public double Indent { get => (double)this.GetValue(TreeViewItem.IndentProperty); set => this.SetValue(TreeViewItem.IndentProperty, value); }
        private bool IsDoubleClickVisible { get; set; }
        /// <summary>Determines if the item is the target of a currently running drag and drop operation.</summary>
        [Bindable(true)]
        public bool IsDraggedOver { get => (bool)this.GetValue(TreeViewItem.IsDraggedOverProperty); set => this.SetValue(TreeViewItem.IsDraggedOverProperty, value); }
        /// <summary>Determines if the item is the currently dragging.</summary>
        public bool IsDragging { get => (bool)this.GetValue(TreeViewItem.IsDraggingProperty); private set => this.SetValue(TreeViewItem.IsDraggingProperty, value); }
        private new TreeViewBase Parent { get; set; }
        /// <summary>Returns the computed left margin of item relative to its root item. This applies to full row selection.</summary>
        [Bindable(true)]
        public Thickness SelectionMargin { get => (Thickness)this.GetValue(TreeViewItem.SelectionMarginProperty); private set => this.SetValue(TreeViewItem.SelectionMarginProperty, value); }
        static TreeViewItem() {
            TreeViewItem.DataContextProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(TreeViewItem.OnDataContextChanged)));
            TreeViewItem.IsDraggedOverProperty = DependencyProperty.Register("IsDraggedOver", typeof(bool), typeof(TreeViewItem), new FrameworkPropertyMetadata(false));
            TreeViewItem.IsDraggingProperty = DependencyProperty.Register("IsDragging", typeof(bool), typeof(TreeViewItem), new FrameworkPropertyMetadata(false));
            TreeViewItem.IndentProperty = DependencyProperty.Register("Indent", typeof(double), typeof(TreeViewItem), new FrameworkPropertyMetadata(0.0));
            TreeViewItem.SelectionMarginProperty = DependencyProperty.Register("SelectionMargin", typeof(Thickness), typeof(TreeViewItem), new FrameworkPropertyMetadata(new Thickness(0.0)));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(typeof(TreeViewItem)));
        }
        /// <summary>Constructs a TreeViewItem.</summary>
        public TreeViewItem() {
            this.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(this.OnLoaded));
        }
        private void Collapse(TreeViewItem item) {
            item.IsExpanded = false;
            for (int i = 0; i < item.Items.Count; i++) {
                TreeViewItem current = (TreeViewItem)item.ItemContainerGenerator.ContainerFromIndex(i);
                if (current != null) {
                    this.Collapse(current);
                }
            }
        }
        /// <summary>Collapses the TreeViewItem control and all its child elements.</summary>
        public void CollapseSubtree() {
            this.Collapse(this);
        }
        private static bool GetAttributes(DependencyObject d, out TreeViewBase parent, out int depth) {
            depth = 0;
            for (ItemsControl i = ItemsControl.ItemsControlFromItemContainer(d); i != null; i = ItemsControl.ItemsControlFromItemContainer(i)) {
                if (i is TreeViewBase view) {
                    parent = view;
                    return true;
                }
                depth++;
            }
            parent = null;
            return false;
        }
        protected override DependencyObject GetContainerForItemOverride() {
            return new TreeViewItem();
        }
        private static bool HasContextMenu(DependencyObject d, out System.Windows.Controls.ContextMenu menu) {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++) {
                DependencyObject o = VisualTreeHelper.GetChild(d, i);
                if (o != null && o is FrameworkElement element && element.ContextMenu != null) {
                    menu = element.ContextMenu;
                    return true;
                }
                if (TreeViewItem.HasContextMenu(o, out menu) && menu != null) {
                    return true;
                }
            }
            menu = null;
            return false;
        }
        protected override bool IsItemItsOwnContainerOverride(object item) {
            return item is TreeViewItem;
        }
        private bool IsOutsideSensitivity(Point point) {
            if (this.Bounds.Contains(point)) {
                return false;
            }
            point.Offset(0d - this.info.Origine.X, 0d - this.info.Origine.Y);
            return Math.Abs(point.X) > Screen.MinimumHorizontalDragDistance || Math.Abs(point.Y) > Screen.MinimumVerticalDragDistance;
        }
        private void NotifyParentToggled() {
            if (this.Parent != null) {
                this.Parent.OnItemToggled(this);
            }
        }
        protected override void OnCollapsed(RoutedEventArgs e) {
            this.NotifyParentToggled();
            base.OnCollapsed(e);
        }
        private void OnDataContextChanged(object value) {
            DoubleClickVisibleAttribute attribute = value?.GetType().GetCustomAttribute<DoubleClickVisibleAttribute>(false);
            this.IsDoubleClickVisible = Assert.NotNull(attribute) && attribute.Value;
        }
        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((TreeViewItem)d).OnDataContextChanged(e.NewValue);
        }
        private void OnDrag(bool value, System.Windows.DragEventArgs e) {
            this.IsDraggedOver = value;
            e.Handled = true;
        }
        protected override void OnDragEnter(System.Windows.DragEventArgs e) {
            this.OnDrag(true, e);
        }
        protected override void OnDragLeave(System.Windows.DragEventArgs e) {
            this.OnDrag(false, e);
        }
        protected override void OnDrop(System.Windows.DragEventArgs e) {
            if (this.Parent != null) {
                this.Parent.OnDragCompleted(this.DataContext ?? (this));
            }
            this.OnDrag(false, e);
        }
        protected override void OnExpanded(RoutedEventArgs e) {
            this.NotifyParentToggled();
            base.OnExpanded(e);
        }
        protected override void OnInitialized(EventArgs e) {
            base.OnInitialized(e);
            if (TreeViewItem.GetAttributes(this, out TreeViewBase parent, out int depth) && this.SetParent(parent)) {
                this.SelectionMargin = new Thickness(this.Indent * (depth + this.Parent.Padding.Left) * -1, 0.0, 0.0, 0.0);
                this.Parent.OnItemAdded(this);
                if (this.IsExpanded) {
                    this.NotifyParentToggled();
                }
            }
        }
        protected override void OnKeyDown(KeyEventArgs e) {
            if (e.Key == Key.Add || e.Key == Key.Subtract) {
                return;
            }
            base.OnKeyDown(e);
        }
        private void OnLoaded(object sender, RoutedEventArgs e) {
            this.ContextMenu = TreeViewItem.HasContextMenu(this, out System.Windows.Controls.ContextMenu menu) ? menu : null;
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);
            if (Mouse.LeftButton == MouseButtonState.Pressed && this.CaptureMouse()) {
                this.info = new DragInfo(PointToScreen(e.GetPosition(this)));
                this.IsDragging = true;
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            if (base.IsMouseCaptured && this.IsDragging) {
                this.ClearValue(TreeViewItem.IsDraggingProperty);
                this.ReleaseMouseCapture();
            }
            base.OnMouseLeftButtonUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (base.IsMouseCaptured && this.IsDragging && this.IsOutsideSensitivity(PointToScreen(e.GetPosition(this))) && !this.info.Started) {
                object obj = this.DataContext ?? (this);
                this.Parent.OnDragStarted(obj);
                DragDrop.DoDragDrop(this, obj, DragDropEffects.Move);
                this.info.Started = true;
            }
        }
        protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e) {
            if (this.IsDoubleClickVisible && this.IsSelected) {
                this.Parent.OnItemDoubleClick(this);
            }
            base.OnPreviewMouseDoubleClick(e);
        }
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e) {
            if (!this.IsSelected) {
                this.IsSelected = true;
            }
            base.OnPreviewMouseRightButtonDown(e);
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo info) {
            base.OnRenderSizeChanged(info);
            this.size = Screen.Convert(info.NewSize);
        }
        private static void Rename(object sender, ExecutedRoutedEventArgs e) {
            if (TemplateItems.FindVisualChild<IEditable>((DependencyObject)sender, out object value)) {
                ((IEditable)value).Edit();
            }
        }
        private bool SetParent(TreeViewBase parent) {
            this.Parent = parent;
            return true;
        }
    }
}
