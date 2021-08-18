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
// A container for a group of controls.
//
// Specific styling is applied to ButtonBase and Separator types.
//
// Targeting ButtonBase for button styles allows to define a single style
// for all inherited types (i.e. ToggleButton, etc.).
//
// ComboBoxes are not styled. The style key only overrides the left and right
// margins. Note that the style targets the System.Windows.Controls.ComboBox 
// base type to allow user-defined controls.
//
// The control internally maintains a collection of descriptor view items
// if IsCustomizable is true. This provides a descriptor view that allows
// user to customize the content of the toolbar. Controls that should be added
// to the descriptor view must implement IDescriptor. Use the visibility
// property to define their default state.
//
// IsCustomizable default value is false.
//
// The header property is used for the tooltip of the overflow button.
// If not specified, the default tooltip is shown.
//
// <ToolBar Header="Standard"> ...
//
// In the above example, the tooltip will be "Standard Toolbar Options".


#pragma warning disable IDE0002

using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Circus.Collections.Observable;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a container for a group of controls.</summary>
    public class ToolBar : System.Windows.Controls.ToolBar {
        /// <summary>Returns the resource key for a button style.</summary>
        public new static readonly ResourceKey ButtonStyleKey;
        /// <summary>Returns the resource key for a combo box style.</summary>
        public new static readonly ResourceKey ComboBoxStyleKey;
        /// <summary>Returns the resource key for a separator style.</summary>
        public new static readonly ResourceKey SeparatorStyleKey;
        /// <summary>Identifies the is customizable dependency property.</summary>
        public static readonly DependencyProperty IsCustomizableProperty;
        /// <summary>Returns a collection of descriptor view items.</summary>
        [Bindable(true)]
        public ObservablePail<DescriptorViewItem> Descriptors { get; private set; }
        /// <summary>Determines if the control is customizable.</summary>
        public bool IsCustomizable { get => (bool)this.GetValue(ToolBar.IsCustomizableProperty); set => this.SetValue(ToolBar.IsCustomizableProperty, value); }
        /// <summary>Returns the overflow popup separator visibility.</summary>
        [Bindable(true)]
        public Visibility SeparatorVisibility { get => this.IsCustomizable && this.HasOverflowItems ? Visibility.Visible : Visibility.Collapsed; }
        static ToolBar() {
            ToolBar.ButtonStyleKey = new ComponentResourceKey(typeof(ToolBar), "ButtonStyleKey");
            ToolBar.ComboBoxStyleKey = new ComponentResourceKey(typeof(ToolBar), "ComboBoxStyleKey");
            ToolBar.SeparatorStyleKey = new ComponentResourceKey(typeof(ToolBar), "SeparatorStyleKey");
            ToolBar.IsCustomizableProperty = DependencyProperty.Register("IsCustomizable", typeof(bool), typeof(ToolBar), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ToolBar.OnIsCustomizableChanged)));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBar), new FrameworkPropertyMetadata(typeof(ToolBar)));
        }
        /// <summary>Constructs a toolbar.</summary>
        public ToolBar() {
        }
        private void Add(int index, UIElement e) {
            if (DescriptorViewItem.Create(e, out DescriptorViewItem d)) {
                this.Descriptors.InsertAt(index, d);
            }
        }
        private static bool Assert<T>(FrameworkElement e) {
            return e is T;
        }
        private static bool GetStyle(FrameworkElement e, out ResourceKey key) {
            key = Assert<ButtonBase>(e) ? ToolBar.ButtonStyleKey : Assert<System.Windows.Controls.ComboBox>(e) ? ToolBar.ComboBoxStyleKey : Assert<Separator>(e) ? ToolBar.SeparatorStyleKey : null;
            return key != null;
        }
        private static void OnIsCustomizableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ToolBar bar = (ToolBar)d;
            if (bar.IsInitialized && (bool)e.NewValue) {
                bar.OnItemsChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
            base.OnItemsChanged(e);
            if (this.IsCustomizable) {
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Add: this.Add(e.NewStartingIndex, (UIElement)e.NewItems[0]); break;
                    case NotifyCollectionChangedAction.Remove: this.Remove(e.OldStartingIndex); break;
                    case NotifyCollectionChangedAction.Replace: this.Replace(e.NewStartingIndex, (UIElement)e.NewItems[0]); break;
                    case NotifyCollectionChangedAction.Reset: this.Reset(); break;
                }
            }
        }
        protected override void PrepareContainerForItemOverride(DependencyObject d, object item) {
            FrameworkElement e = (FrameworkElement)d;
            if (ToolBar.GetStyle(e, out ResourceKey key)) {
                e.SetResourceReference(FrameworkElement.StyleProperty, key);
            }
        }
        private void Remove(int index) {
            this.Descriptors.RemoveAt(index);
        }
        private void Replace(int index, UIElement e) {
            if (DescriptorViewItem.Create(e, out DescriptorViewItem d)) {
                this.Descriptors[index] = d;
            }
        }
        private void Reset() {
            this.Descriptors = ObservablePail<DescriptorViewItem>.Create<UIElement>(this.Items.SourceCollection, DescriptorViewItem.Create);
        }
    }
}
