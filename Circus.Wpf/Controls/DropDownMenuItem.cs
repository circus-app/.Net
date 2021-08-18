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
// A drop-down menu item.


#pragma warning disable IDE0002

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Circus.Wpf.Input;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a drop-down menu item.</summary>
    public class DropDownMenuItem : System.Windows.Controls.MenuItem {
        private new DropDownMenu Parent => (DropDownMenu)ItemsControl.ItemsControlFromItemContainer(this);
        static DropDownMenuItem() {
            DropDownMenuItem.CommandProperty.OverrideMetadata(typeof(DropDownMenuItem), new FrameworkPropertyMetadata(null, DropDownMenuItem.OnCommandChanged));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownMenuItem), new FrameworkPropertyMetadata(typeof(DropDownMenuItem)));
        }
        /// <summary>Constructs a drop-down menu item.</summary>
        public DropDownMenuItem() { 
        }
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            RelayCommand.RegisterContext(d, e.NewValue);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            if (Assert.NotNull(this.Parent)) {
                this.Parent.Close();
            }
            base.OnMouseLeftButtonUp(e);
        }
    }
}
