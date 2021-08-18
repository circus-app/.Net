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
// A container for side menus and a content.
//
// Provides a container to host side menus using the left and right properties, 
// and a content placed on the center panel. The control also provides resize
// grips for both left and right panels.
//
// Side menus can be added either using the TypeBinding extension if they are
// defined in a specific file or, using the property setter tag like this:
//
// <SideMenuPanel>
//    <SideMenuPanel.Left>
//        <SideMenu>
//            <SideMenuItem Header="...">
//                ...
//            <SideMenuItem>
//        </SideMenu>
//    </SideMenuPanel.Left>
//    <SideMenuPanel.Right>
//        <SideMenu>
//            <SideMenuItem Header="...">
//                ...
//            </SideMenuItem>
//        </SideMenu>
//    </SideMenuPanel.Right>
//    ...
// </SideMenuPanel>
//
// Docking is automatically adjusted depending on panel the side menu is placed
// on which makes specifying this property useless. For most cases, side menus 
// are intended to occupy the left and/or right edges of their parent container
// (usually a window). Therefore, left and right margins are 0. Nevertheless, 
// these values are not checked nor overriden, which allows more complex cases
// (i.e. nesting panels, etc.).
//
// Side menus can be removed through code setting their corresponding panel value
// to null.


#pragma warning disable IDE0002

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a container for side menus and a content.</summary>
    public class SideMenuPanel : ContentControl {
        /// <summary>Identifies the left dependency property.</summary>
        public static readonly DependencyProperty LeftProperty;
        /// <summary>Identifies the right dependency property.</summary>
        public static readonly DependencyProperty RightProperty;
        /// <summary>Returns the resource key for the internal grid splitter style.</summary>
        public static readonly ResourceKey SplitStyleKey;
        /// <summary>Gets or sets the side menu placed on the left panel.</summary>
        [Bindable(true)]
        public SideMenu Left { get => (SideMenu)this.GetValue(SideMenuPanel.LeftProperty); set => this.SetValue(SideMenuPanel.LeftProperty, value); }
        /// <summary>Gets or sets the side menu placed in the right panel.</summary>
        [Bindable(true)]
        public SideMenu Right { get => (SideMenu)this.GetValue(SideMenuPanel.RightProperty); set => this.SetValue(SideMenuPanel.RightProperty, value); }
        static SideMenuPanel() {
            SideMenuPanel.LeftProperty = DependencyProperty.Register("Left", typeof(SideMenu), typeof(SideMenuPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(SideMenuPanel.OnLeftChanged)), SideMenuPanel.IsValidSideMenu);
            SideMenuPanel.RightProperty = DependencyProperty.Register("Right", typeof(SideMenu), typeof(SideMenuPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(SideMenuPanel.OnRightChanged)), SideMenuPanel.IsValidSideMenu);
            SideMenuPanel.SplitStyleKey = new ComponentResourceKey(typeof(SideMenuPanel), "SplitStyleKey");
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SideMenuPanel), new FrameworkPropertyMetadata(typeof(SideMenuPanel)));
        }
        /// <summary>Constructs a side menu panel.</summary>
        public SideMenuPanel() { 
        }
        private static bool IsValidSideMenu(object value) {
            return value == null || value is SideMenu;
        }
        private static void OnLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != null) {
                ((SideMenu)e.NewValue).Dock = Dock.Left;
            }
        }
        private static void OnRightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != null) {
                SideMenu menu = (SideMenu)e.NewValue;
                menu.SetValue(Grid.ColumnProperty, 4);
                menu.Dock = Dock.Right;
            }
        }
    }
}
