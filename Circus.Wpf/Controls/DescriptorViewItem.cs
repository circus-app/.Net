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
// An item inside a descriptor view.
//
// The static creator takes an UIElement as input and creates a descriptor 
// view item if IDescriptor is implemented and its description is not null. 
// Other IDescriptor properties are optional. This is mostly intended to
// be used as a predicate.


#pragma warning disable IDE0002

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Circus.Wpf.Controls {
    /// <summary>Provides an item inside a descriptor view.</summary>
    public sealed class DescriptorViewItem : Control, IDescriptor {
        private readonly IDescriptor descriptor;
        private readonly UIElement element;
        private readonly Visibility visibility;
        /// <summary>Identifies the is checked dependency property.</summary>
        public static readonly DependencyProperty IsCheckedProperty;
        /// <summary>Identifies the is highlighted dependency property.</summary>
        public static readonly DependencyProperty IsHighlightedProperty;
        public string Description => this.descriptor?.Description;
        public object Icon => this.descriptor?.Icon;
        /// <summary>Returns true if the item is checked.</summary>
        [Bindable(true)]
        public bool IsChecked { get =>(bool) this.GetValue(DescriptorViewItem.IsCheckedProperty); private set => this.SetValue(DescriptorViewItem.IsCheckedProperty, value); }
        /// <summary>Returns true if the item is highlighted.</summary>
        [Bindable(true)]
        public bool IsHighlighted { get =>(bool)this.GetValue(DescriptorViewItem.IsHighlightedProperty); private set => this.SetValue(DescriptorViewItem.IsHighlightedProperty, value); }
        public string InputGestureText => this.descriptor?.InputGestureText;
        static DescriptorViewItem() {
            DescriptorViewItem.IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(DescriptorViewItem), new FrameworkPropertyMetadata(false));
            DescriptorViewItem.IsHighlightedProperty = DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(DescriptorViewItem), new FrameworkPropertyMetadata(false));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DescriptorViewItem), new FrameworkPropertyMetadata(typeof(DescriptorViewItem)));
        }
        /// <summary>Constructs a descriptor view item.</summary>
        public DescriptorViewItem() {
            this.DataContext = this;
        }
        private DescriptorViewItem(IDescriptor descriptor, UIElement element) : this() {
            this.descriptor = descriptor;
            this.element = element;
            this.visibility = element.Visibility;
            element.IsVisibleChanged += this.OnIsVisibleChanged;
        }
        /// <summary>Returns a descriptor view item from the provided UIElement. Returns true if e is IDescriptor and description is not null.</summary>
        public static bool Create(UIElement e, out DescriptorViewItem d) {
            d = Assert.Is(e, out IDescriptor descriptor) && !string.IsNullOrEmpty(descriptor.Description) ? new DescriptorViewItem(descriptor, e) : null;
            return Assert.NotNull(d);
        }
        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            this.IsChecked = (bool)e.NewValue;
        }
        protected override void OnMouseEnter(MouseEventArgs e) {
            base.OnMouseEnter(e);
            this.IsHighlighted = true;
        }
        protected override void OnMouseLeave(MouseEventArgs e) {
            base.OnMouseLeave(e);
            this.IsHighlighted = false;
        }
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e) {
            this.element.Visibility = this.element.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            e.Handled = true;
            base.OnPreviewMouseLeftButtonDown(e);
        }
        /// <summary>Resets the visibility of the target control to its original value.</summary>
        public void Reset() {
            this.element.Visibility = this.visibility;
        }
    }
}