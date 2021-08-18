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
// A selectable item in a ListBox.
//
// Like any other selectable control, the ListBoxItem notifies its parent 
// ListBox when mouse click occurs. This allows to distinguish between user 
// interaction and programmatic selection changes.
// 
// Since ListBox supports DataSource binding, double-clicking is also
// reported.


#pragma warning disable IDE0002

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a selectable item in a ListBox.</summary>
    public class ListBoxItem : System.Windows.Controls.ListBoxItem {
        private new ListBox Parent => (ListBox)ItemsControl.ItemsControlFromItemContainer(this);
        static ListBoxItem() {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ListBoxItem), new FrameworkPropertyMetadata(typeof(ListBoxItem)));
        }
        public ListBoxItem() { 
        }
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e) {
            if ((e.Source == this || !this.IsSelected) && this.Focus()) {
                e.Handled = true;
            }
            this.Parent.NotifyItemDoubleClick(this);
            base.OnMouseDoubleClick(e);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            if ((e.Source == this || !this.IsSelected) && this.Focus()) {
                e.Handled = true;
            }
            this.Parent.NotifyItemClick(this);
            base.OnMouseLeftButtonUp(e);
        }
    }
}
