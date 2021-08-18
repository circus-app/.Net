#pragma warning disable IDE0002

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Circus.Collections.Observable;
using Circus.Runtime;
using Circus.Wpf.Data;
namespace Circus.Wpf.Controls {
    [ClassCommand("CloseAll")]
    [ClassCommand("CloseNotSelected")]
    [ClassCommand("CloseNotToggled")]
    [ClassCommand("FloatAll")]
    [TemplatePart(Name = "Panel", Type = typeof(DocumentTrayPanel))]
    public class DocumentTray : Primitives.Tab {
        public static readonly ResourceKey ContextMenuKey;
        public static readonly DependencyProperty HeaderProperty;
        public DocumentTrayItemsSource Children { get; internal set; }
        private DocumentTrayDragInfo DragInfo { get; set; }
        [Bindable(true)]
        public object Header { get => base.GetValue(DocumentTray.HeaderProperty); set => base.SetValue(DocumentTray.HeaderProperty, value); }
        public Window Owner { get; private set; }
        private DocumentTrayPanel Panel { get; set; }
        [Bindable(true)]
        public ObservablePail<SelectorViewItem> Selectors { get; private set; }
        public DocumentTrayStackInfo StackInfo { get; private set; }
        static DocumentTray() {
            DocumentTray.ContextMenuKey = new ComponentResourceKey(typeof(DocumentTray), "ContextMenuKey");
            DocumentTray.HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(DocumentTray), new FrameworkPropertyMetadata("Documents"));
            DocumentTray.SelectedItemProperty.OverrideMetadata(typeof(DocumentTray), new FrameworkPropertyMetadata(Boxes.Null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentTray), new FrameworkPropertyMetadata(typeof(DocumentTray)));
        }
        public DocumentTray() {
            this.StackInfo = new DocumentTrayStackInfo();
        }
        private void Add(object item) {
            if (Assert.As(base.ItemContainerGenerator.ContainerFromItem(item), out ISelector selector) && SelectorViewItem.Create(selector, out SelectorViewItem view)) {
                this.Selectors.Insert(this.StackInfo.Next, view);
            }
            base.SelectedIndex = this.StackInfo.Next;
        }
        private void Close(int index) {
            for (int i = this.Children.Items.Count - 1; i >= index; i--) {
                base.DataSource.Invoke(base.DataSource.OnItemRemoved, this, new DataEventArgs(this.Children.Items[i]));
            }
        }
        public void CloseAll() {
            this.Close(0);
            this.StackInfo.Reset();
        }
        private static void CloseAll(object sender, ExecutedRoutedEventArgs e) {
            if (Assert.As(sender, out DocumentTray control)) {
                control.Close(0);
            }
        }
        public void CloseNotSelected() {
            if (base.SelectedIndex > -1 && this.Children.Items.Count > 1) {
                if (base.SelectedIndex > 0 && Assert.As(base.ItemContainerGenerator.ContainerFromIndex(base.SelectedIndex), out Document document) && Allocator.Assign(document.IsToggled, out bool num)) {
                    this.Children.Move(base.SelectedItem, base.SelectedIndex, 0);
                    if (num && Assert.As(base.ItemContainerGenerator.ContainerFromIndex(0), out document)) {
                        document.Toggle(true);
                        this.StackInfo.Reset(1);
                    }
                }
                this.Close(1);
            }
        }
        private static void CloseNotSelected(object sender, ExecutedRoutedEventArgs e) {
            if (Assert.As(sender, out DocumentTray control)) {
                control.CloseNotSelected();
            }
        }
        public void CloseNotToggled() {
            if (!this.StackInfo.Empty) {
                this.Close(this.StackInfo.Next);
            }
        }
        private static void CloseNotToggled(object sender, ExecutedRoutedEventArgs e) {
            if (Assert.As(sender, out DocumentTray control)) {
                control.CloseNotToggled();
            }
        }
        public IEnumerable<T> Containers<T>() {
            for (int i = 0; i < base.Items.Count; i++) {
                yield return Assert.As(base.ItemContainerGenerator.ContainerFromItem(i), out T value) ? value : default;
            }
        }
        private bool Find(Point point, out Duple<int> info) {
            for (int i = 0; i < this.Children.Items.Count; i++) {
                if (Assert.As(this.Children.Items[i], out object item) && item != this.DragInfo.Item && Assert.As(base.ItemContainerGenerator.ContainerFromItem(item), out Document document) && document.Bounds.Contains(point) && this.Children.Items.Contains(this.DragInfo.Item, out int index) && Allocator.Assign(new Duple<int>(index, i), out info)) {
                    return true;
                }
            }
            return !Allocator.Assign(null, out info);
        }
        public void FloatAll() {
            for (int i = this.Children.Items.Count - 1; i >= 0; i--) {
                if (Allocator.Assign(this.Children.Items[i], out object item) && Assert.As(base.ItemContainerGenerator.ContainerFromItem(item), out Document document)) {
                    this.Children.Items.Remove(i);
                    DockManager.Commit(this, item, document.IsToggled);
                }
            }
            this.StackInfo.Reset();
        }
        private static void FloatAll(object sender, ExecutedRoutedEventArgs e) {
            if (Assert.As(sender, out DocumentTray control)) {
                control.FloatAll();
            }
        }
        protected override DependencyObject GetContainerForItemOverride() {
            return new Document();
        }
        protected override bool IsItemItsOwnContainerOverride(object item) {
            return Assert.Is<Document>(item);
        }
        internal void NotifyClose(object item) {
            if (Assert.As(base.ItemContainerGenerator.ContainerFromItem(item), out Document document)) {
                this.StackInfo.Update(document.IsToggled);
            }
            base.DataSource.Invoke(base.DataSource.OnItemRemoved, this, new DataEventArgs(item));
        }
        internal void NotifyDrag(Point point) {
            if (this.DragInfo.IsOutsideSensitivity(point) && this.Find(point, out Duple<int> info) && this.StackInfo.IsDrop(info.Second, this.DragInfo.Toggled, out bool update)) {
                this.Children.Move(this.DragInfo.Item, info.First, info.Second);
                if (Assert.As(base.ItemContainerGenerator.ContainerFromIndex(info.Second), out Document document)) {
                    if (update) {
                        document.Toggle(true);
                    }
                    this.DragInfo.Update(point, document.Bounds);
                }
                base.SelectedIndex = info.Second;
            }
        }
        internal void NotifyDragCompleted() {
            if (Assert.NotNull(this.DragInfo)) {
                this.DragInfo.Dispose();
            }
        }
        internal void NotifyDragStarted(object item) {
            if (Assert.As(base.ItemContainerGenerator.ContainerFromItem(item), out Document document)) {
                this.DragInfo = new DocumentTrayDragInfo(item, document.Bounds, document.IsToggled);
            }
        }
        internal void NotifyFloat(object item, bool toggled) {
            if (DockManager.Request(this) && this.Children.Items.Contains(item, out int index)) {
                this.StackInfo.Update(toggled);
                this.Children.Items.Remove(index);
                DockManager.Commit(this, item, toggled);
            }
        }
        internal void NotifyToggled(object item) {
            if (Assert.As(base.ItemContainerGenerator.ContainerFromItem(item), out Document document) && Allocator.Assign(document.IsToggled, out bool num) && this.Children.Items.Contains(item, out int index) && this.StackInfo.IsMoved(index, num, out int position)) {
                this.Children.Move(item, index, position);
                if (num && Assert.As(base.ItemContainerGenerator.ContainerFromIndex(position), out document)) {
                    document.Toggle(true);
                }
            }
            else {
                this.Panel.Invalidate();
            }
        }
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            TemplateItems.Register(this);
        }
        protected override void OnIsVisibleChanged(bool value) {
            base.OnIsVisibleChanged(value);
            if (!value) {
                if (Assert.NotNull(this.Owner) && DockManager.Unregister(this)) {
                    this.Owner = null;
                }
                if (Assert.NotNull(this.Children)) {
                    this.Children.Dispose();
                }
            }
        }
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
            if (!this.Children.Locked) {
                base.OnItemsChanged(e);
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Add: this.Add(e.NewItems[0]); break;
                    case NotifyCollectionChangedAction.Remove: this.Remove(e.OldStartingIndex); break;
                    case NotifyCollectionChangedAction.Replace: this.Replace(e.NewStartingIndex); break;
                    case NotifyCollectionChangedAction.Reset: this.Reset(); break;
                }
            }
        }
        protected override void OnItemsSourceChanged(object value) {
            if (Allocator.Assign(Assert.NotNull(this.Children), out bool num) && num && Assert.Is(value, out IObservable observable) && observable == this.Children.Items) {
                return;
            }
            if (num) {
                this.Children.Dispose();
            }
            this.Children = new DocumentTrayItemsSource((IObservable)value, this.StackInfo);
            base.SetCurrentValue(ItemsControl.ItemsSourceProperty, this.Children.Items);
        }
        protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
            if (base.SelectedIndex > -1 && Assert.NotNull(this.Panel) && this.Panel.IsOutsideVisibility(base.SelectedIndex)) {
                this.Children.Move(base.SelectedItem, base.SelectedIndex, this.StackInfo.Next);
            }
            base.OnSelectionChanged(e);
        }
        protected override void OnVisualParentChanged(DependencyObject parent) {
            base.OnVisualParentChanged(parent);
            if (Assert.NotNull(this.Owner)) {
                DockManager.Unregister(this);
            }
            this.Owner = Window.GetWindow(this);
            if (Assert.NotNull(this.Owner)) {
                DockManager.Register(this);
            }
        }
        private void Remove(int index) {
            this.Selectors.Remove(index);
        }
        private void Replace(int index) {
            if (Assert.As(base.ItemContainerGenerator.ContainerFromIndex(index), out ISelector selector) && SelectorViewItem.Create(selector, out SelectorViewItem item) && this.Selectors.RemoveAt(index)) {
                this.Selectors.Add(item);
            }
        }
        private void Reset() {
            this.Selectors = ObservablePail<SelectorViewItem>.Create<ISelector>(this.Containers<ISelector>(), SelectorViewItem.Create);
        }
    }
}