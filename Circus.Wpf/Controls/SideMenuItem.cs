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
// An item inside a side menu.


#pragma warning disable IDE0002

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
namespace Circus.Wpf.Controls {
    /// <summary>Provides an item inside a side menu.</summary>
    public class SideMenuItem : HeaderedContentControl {
        /// <summary>Identifies the is selected dependency property.</summary>
        public static readonly DependencyProperty IsSelectedProperty;
        /// <summary>Returns the resource key for a templated menu item docked to the left.</summary>
        public static readonly ResourceKey LeftTemplateKey;
        /// <summary>Returns the resource key for a templated menu item docked to the right.</summary>
        public static readonly ResourceKey RightTemplateKey;
        /// <summary>Determines if the item is currently selected.</summary>
        [Bindable(true)]
        public bool IsSelected { get => (bool)this.GetValue(SideMenuItem.IsSelectedProperty); set => this.SetValue(SideMenuItem.IsSelectedProperty, value); }
        private new SideMenu Parent => (SideMenu)ItemsControl.ItemsControlFromItemContainer(this);
        static SideMenuItem() {
            SideMenuItem.IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(SideMenuItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            SideMenuItem.LeftTemplateKey = new ComponentResourceKey(typeof(SideMenuItem), "LeftTemplateKey");
            SideMenuItem.RightTemplateKey = new ComponentResourceKey(typeof(SideMenuItem), "RightTemplateKey");
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SideMenuItem), new FrameworkPropertyMetadata(typeof(SideMenuItem)));
        }
        /// <summary>Constructs a side menu item.</summary>
        public SideMenuItem() {
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            if ((e.Source == this || !this.IsSelected) && this.Focus()) {
                e.Handled = true;
            }
            this.Parent.NotifyItemClicked(this);
            base.OnMouseLeftButtonDown(e);
        }
    }
}
