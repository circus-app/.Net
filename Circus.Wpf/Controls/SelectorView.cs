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
// A pop-up menu that exposes a collection of SelectorViewItem.
//
// Items are sorted alphabetically.


#pragma warning disable IDE0002

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Circus.Runtime;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a pop-up menu that exposes a collection of SelectorViewItem.</summary>
    public class SelectorView : ContextMenu {
        /// <summary>Identifies the sort dependency property.</summary>
        public static readonly DependencyProperty SortProperty;
        /// <summary>Gets or sets the SortDescription used to sort items.</summary>
        [Bindable(true)]
        public SortDescription Sort { get => (SortDescription)this.GetValue(SelectorView.SortProperty); set => this.SetValue(SelectorView.SortProperty, value); }
        static SelectorView() {
            SelectorView.SortProperty = DependencyProperty.Register("Sort", typeof(SortDescription), typeof(SelectorView), new FrameworkPropertyMetadata(new SortDescription(), new PropertyChangedCallback(SelectorView.OnSortChanged)));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectorView), new FrameworkPropertyMetadata(typeof(SelectorView)));
        }
        /// <summary>Constructs a SelectorView.</summary>
        public SelectorView() {
            this.Items.IsLiveSorting = true;
        }
        private void AddSort(SortDescription description) {
            this.Items.SortDescriptions.Add(description);
            this.Items.LiveSortingProperties.Add(description.PropertyName);
        }
        protected override DependencyObject GetContainerForItemOverride() {
            return new SelectorViewItem();
        }
        /// <summary>Returns the parent selector dependency object.</summary>
        protected virtual Selector GetParentSelectorOverride() {
            return Assert.Is(base.TemplatedParent, out Selector selector) ? selector : null;
        }
        protected override bool IsItemItsOwnContainerOverride(object item) {
            return Assert.Is<SelectorViewItem>(item);
        }
        internal void NotifyClose(object item) {

            // Release mouse capture when items is empty to ensure capture is not 
            // kept on an item that is no longer accessible and therefore prevents
            // other controls from receiving keyboard focus.
            if (base.Items.Count == 1) {
                base.ReleaseMouseCapture();
            }
            if (Allocator.Assign(this.GetParentSelectorOverride(), out Selector parent) && Assert.NotNull(parent) && Assert.Is(parent.ItemContainerGenerator.ContainerFromItem(item), out ISelector selector)) {
                selector.Close();
            }
        }
        internal void NotifySelect(object item) {
            if (Allocator.Assign(this.GetParentSelectorOverride(), out Selector parent) && Assert.NotNull(parent)) {
                parent.SelectedItem = item;
            }
        }
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            // Sets ItemsSource binding to parent Selectors. Xaml binding fails if 
            // Selectors are null when parent instance is created. This is the case 
            // in floating windows trays since items source is bound lately. 
            if (Assert.As(this.GetParentSelectorOverride(), out DocumentTray parent)) {
                base.ItemsSource = parent.Selectors;
            }
        }
        private void OnSortChanged(object value) {
            if (this.Items.SortDescriptions.Count == 1) {
                this.Items.SortDescriptions.RemoveAt(0);
            }
            if (Assert.NotNull(value) && Assert.As(value, out SortDescription description)) {
                this.AddSort(description);
            }
        }
        private static void OnSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (Assert.As(d, out SelectorView view)) {
                view.OnSortChanged(e.NewValue);
            }
        }
    }
}
