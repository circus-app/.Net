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
// A selectable item inside a menu.


#pragma warning disable IDE0002

using System.Windows;
using Circus.Wpf.Input;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a selectable item inside a menu.</summary>
    public class MenuItem : System.Windows.Controls.MenuItem {
        static MenuItem() {
            MenuItem.CommandProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(null, MenuItem.OnCommandChanged));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(typeof(MenuItem)));
        }
        /// <summary>Constructs a window menu item.</summary>
        public MenuItem() {
        }
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            RelayCommand.RegisterContext(d, e.NewValue);
        }
    }
}
