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
// An item inside a SelectorView.


#pragma warning disable IDE0002

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Circus.Runtime;
namespace Circus.Wpf.Controls {
    /// <summary>Provides an item inside a SelectorView.</summary>
    [ClassCommand("Close")]
    [ClassCommand("Select")]
    public class SelectorViewItem : System.Windows.Controls.MenuItem {
        /// <summary>Identifies the resource key for a button style.</summary>
        public static readonly ResourceKey ButtonStyleKey;
        private new SelectorView Parent => (SelectorView)ItemsControl.ItemsControlFromItemContainer(this);
        static SelectorViewItem() {
            SelectorViewItem.ButtonStyleKey = new ComponentResourceKey(typeof(SelectorViewItem), "ButtonStyleKey");
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectorViewItem), new FrameworkPropertyMetadata(typeof(SelectorViewItem)));
        }
        internal SelectorViewItem() {
        }
        private SelectorViewItem(ISelector selector) {
            this.DataContext = selector.DataContext;
            this.Header = selector.Header;
            this.Icon = selector.Icon;
        }
        /// <summary>Outputs a selector view item from the provided ISelector. Returns true if creation succeeded.</summary>
        public static bool Create(ISelector selector, out SelectorViewItem item) {
            return Allocator.Assign(new SelectorViewItem(selector), out item);
        }
        private static void Close(object sender, ExecutedRoutedEventArgs e) {
            if (Assert.As(sender, out SelectorViewItem item)) {
                item.Parent.NotifyClose(item.DataContext);
            }
        }
        private static void Select(object sender, ExecutedRoutedEventArgs e) {
            if (Assert.As(sender, out SelectorViewItem item)) {
                item.Parent.NotifySelect(item.DataContext);
            }
        }
    }
}
