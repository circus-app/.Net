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
// A presenter for a collection of IDescriptor controls.


#pragma warning disable IDE0002

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a presenter for a collection of IDescriptor controls.</summary>
    [ClassCommand("Reset")]
    public sealed class DescriptorView : ItemsControl {
        /// <summary>Returns the resource key for a menu item style.</summary>
        public static readonly ResourceKey MenuItemStyleKey;
        static DescriptorView() {
            DescriptorView.MenuItemStyleKey = new ComponentResourceKey(typeof(DescriptorView), "MenuItemStyleKey");
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DescriptorView), new FrameworkPropertyMetadata(typeof(DescriptorView)));
        }
        /// <summary>Constructs a descriptor view.</summary>
        public DescriptorView() {
        }
        protected override DependencyObject GetContainerForItemOverride() {
            return new DescriptorViewItem();
        }
        protected override bool IsItemItsOwnContainerOverride(object item) {
            return Assert.Is<DescriptorViewItem>(item);
        }
        private static void Reset(object sender, ExecutedRoutedEventArgs e) {
            DescriptorView control = (DescriptorView)sender;
            foreach (DescriptorViewItem i in control.Items) {
                i.Reset();
            }
            if (Assert.Is(control.TemplatedParent, out ToolBar bar)) {
                bar.IsOverflowOpen = false;
            }
        }
    }
}
