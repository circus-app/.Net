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
// A textbox for a search control.
//
// The focused property is here to provide a two way binding between the parent
// search and the current textbox and therefore avoids registering control.
//
// IsEmpty controls the background of the textbox. It determines the visibility
// of the parent watermark.


#pragma warning disable IDE0002

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Circus.Wpf.Controls {
    /// <summary>Provides textbox for a search control.</summary>
    public class SearchTextBox : TextBox {
        /// <summary>Identifies the focused dependency property.</summary>
        public static readonly DependencyProperty FocusedProperty;
        /// <summary>Identifies the is empty dependency property.</summary>
        public static readonly DependencyProperty IsEmptyProperty;
        /// <summary>Determines if the control is focused.</summary>
        public bool Focused { get => (bool)this.GetValue(SearchTextBox.FocusedProperty); set => this.SetValue(SearchTextBox.FocusedProperty, value); }
        /// <summary>Determines if the text contents is empty.</summary>
        public bool IsEmpty { get => (bool)this.GetValue(SearchTextBox.IsEmptyProperty); private set => this.SetValue(SearchTextBox.IsEmptyProperty, value); }
        static SearchTextBox() {
            SearchTextBox.FocusedProperty = DependencyProperty.Register("Focused", typeof(bool), typeof(SearchTextBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(SearchTextBox.OnFocusedChanged)));
            SearchTextBox.IsEmptyProperty = DependencyProperty.Register("IsEmpty", typeof(bool), typeof(SearchTextBox), new FrameworkPropertyMetadata(true));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchTextBox), new FrameworkPropertyMetadata(typeof(SearchTextBox)));
        }
        /// <summary>Constructs a search textbox.</summary>
        public SearchTextBox() {
        }
        private static void OnFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ((bool)e.NewValue) {
                ((SearchTextBox)d).Focus();
            }
        }
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            if (!string.IsNullOrEmpty(base.Text)) {
                this.SelectAll();
            }
            base.OnGotKeyboardFocus(e);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e) {
            if (!this.IsKeyboardFocusWithin) {
                this.Focus();
                e.Handled = true;
            }
            base.OnMouseDown(e);
        }
        protected override void OnTextChanged(TextChangedEventArgs e) {
            this.IsEmpty = this.Text.Length == 0;
            base.OnTextChanged(e);
        }
    }
}
