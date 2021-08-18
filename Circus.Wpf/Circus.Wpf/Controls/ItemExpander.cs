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
// An expander for an expandable item control.


#pragma warning disable IDE0002

using System.Windows;
namespace Circus.Wpf.Controls {
    /// <summary>Provides an expander for an expandable item control.</summary>
    public class ItemExpander : ToggleButton {
        /// <summary>Identifies the focused property.</summary>
        public static readonly DependencyProperty FocusedProperty;
        /// <summary>Identifies the is selected property.</summary>
        public static readonly DependencyProperty IsSelectedProperty;
        /// <summary>Determines if the control contains focus.</summary>
        public bool Focused { get => (bool)this.GetValue(ItemExpander.FocusedProperty); internal set => this.SetValue(ItemExpander.FocusedProperty, value); }
        /// <summary>Determines if the control is selected.</summary>
        public bool IsSelected { get => (bool)this.GetValue(ItemExpander.IsSelectedProperty); internal set => this.SetValue(ItemExpander.IsSelectedProperty, value); }
        static ItemExpander() {
            ItemExpander.FocusedProperty = DependencyProperty.Register("Focused", typeof(bool), typeof(ItemExpander), new FrameworkPropertyMetadata(false));
            ItemExpander.IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ItemExpander), new FrameworkPropertyMetadata(false));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemExpander), new FrameworkPropertyMetadata(typeof(ItemExpander)));
        }
        /// <summary>Constructs an ItemExpander.</summary>
        public ItemExpander() {
        }
    }
}
