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
// A toolbar menu that implements IDescriptor.
//
// The button part executes the first (topmost) item command if defined.
//
// Since icon is added to the header property, caption should be set
// in the text property.


#pragma warning disable IDE0002

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a toolbar menu that implements IDescriptor.</summary>
    public class DropDownMenu : HeaderedItemsControl, IDescriptor {
        /// <summary>Identifies the description dependency property.</summary>
        public static readonly DependencyProperty DescriptionProperty;
        /// <summary>Identifies the icon dependency property.</summary>
        public static readonly DependencyProperty IconProperty;
        /// <summary>Identifies the input gesture text dependency property.</summary>
        public static readonly DependencyProperty InputGestureTextProperty;
        /// <summary>Identifies the is drop-down open dependency property.</summary>
        public static readonly DependencyProperty IsDropDownOpenProperty;
        /// <summary>Identifies the is pressed dependency property.</summary>
        public static readonly DependencyProperty IsPressedProperty;
        /// <summary>Identifies the text dependency property.</summary>
        public static readonly DependencyProperty TextProperty;
        /// <summary>Gets or sets the user-friendly description of the control.</summary>
        public string Description { get => (string)this.GetValue(DropDownMenu.DescriptionProperty); set => this.SetValue(DropDownMenu.DescriptionProperty, value); }
        /// <summary>Gets or sets the icon of the control.</summary>
        public object Icon { get => this.GetValue(DropDownMenu.IconProperty); set => this.SetValue(DropDownMenu.IconProperty, value); }
        /// <summary>Gets or sets the shortcut text if access key is specified.</summary>
        public string InputGestureText { get => (string)this.GetValue(DropDownMenu.InputGestureTextProperty); set => this.SetValue(DropDownMenu.InputGestureTextProperty, value); }
        /// <summary>Returns true if the drop-down part is opened.</summary>
        public bool IsDropDownOpen { get => (bool)this.GetValue(DropDownMenu.IsDropDownOpenProperty); set => this.SetValue(DropDownMenu.IsDropDownOpenProperty, value); }
        /// <summary>Returns true if the button part is pressed.</summary>
        public bool IsPressed { get => (bool)this.GetValue(DropDownMenu.IsPressedProperty); private set => this.SetValue(DropDownMenu.IsPressedProperty, value); }
        /// <summary>Gets or sets the text of the control.</summary>
        public string Text { get => (string)this.GetValue(DropDownMenu.TextProperty); set => this.SetValue(DropDownMenu.TextProperty, value); }
        static DropDownMenu() {
            DropDownMenu.DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(DropDownMenu), new FrameworkPropertyMetadata(null));
            DropDownMenu.IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(DropDownMenu), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DropDownMenu.OnIconChanged)));
            DropDownMenu.InputGestureTextProperty = DependencyProperty.Register("InputGestureText", typeof(string), typeof(DropDownMenu), new FrameworkPropertyMetadata(null));
            DropDownMenu.IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(DropDownMenu), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(DropDownMenu.OnIsDropDownOpenChanged)));
            DropDownMenu.IsPressedProperty = DependencyProperty.Register("IsPressed", typeof(bool), typeof(DropDownMenu), new FrameworkPropertyMetadata(false));
            DropDownMenu.TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DropDownMenu), new FrameworkPropertyMetadata(null));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownMenu), new FrameworkPropertyMetadata(typeof(DropDownMenu)));
        }
        /// <summary>Constructs a drop-down menu.</summary>
        public DropDownMenu() {
        }
        internal void Close() {
            this.SetValue(DropDownMenu.IsDropDownOpenProperty, false);
        }
        protected override DependencyObject GetContainerForItemOverride() {
            return new DropDownMenuItem();
        }
        private bool GetItem(out DropDownMenuItem item) {
            item = this.Items.Count > 0 ? (DropDownMenuItem)this.Items[0] : null;
            return Assert.NotNull(item);
        }
        protected override bool IsItemItsOwnContainerOverride(object item) {
            return Assert.Is<DropDownMenuItem>(item);
        }
        private void OnClick() {
            if (this.GetItem(out DropDownMenuItem item) && Assert.NotNull(item.Command)) {
                item.Command.Execute(item.CommandParameter);
            }
        }
        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            d.SetValue(DropDownMenu.HeaderProperty, ResourceManager.Get(e.NewValue, out object value) ? value : e.NewValue);
        }
		private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            DropDownMenu menu = (DropDownMenu)d;
            if ((bool)e.NewValue) {
                Mouse.Capture(menu, CaptureMode.SubTree);
            }
            else {
                if (menu.IsKeyboardFocusWithin) {
                    menu.Focus();
                }
                if (Mouse.Captured == menu) {
                    Mouse.Capture(null);
                }
            }
		}
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
            base.OnIsKeyboardFocusWithinChanged(e);
            if (this.IsDropDownOpen && !base.IsKeyboardFocusWithin) {
                DependencyObject d = (DependencyObject)Keyboard.FocusedElement;
                if (Assert.Null(d) || ItemsControl.ItemsControlFromItemContainer(d) != this) {
                    this.Close();
                }
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            if (!this.IsDropDownOpen) {
                if (this.IsMouseOver) {
                    this.SetValue(DropDownMenu.IsPressedProperty, true);
                }
                else if (!this.IsKeyboardFocusWithin) {
                    this.Focus();
                }
            }
            e.Handled = true;
            if (Mouse.Captured == this && e.OriginalSource == this) {
                this.Close();
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            e.Handled = true;
            this.SetValue(DropDownMenu.IsPressedProperty, false);
            this.OnClick();
        }
    }
}
