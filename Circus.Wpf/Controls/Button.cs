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
// A button that implements IDescriptor.
//
// Since icon is added to the content property, caption should be set in 
// the text property.


#pragma warning disable IDE0002

using System.Windows;
using Circus.Wpf.Input;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a button that implements IDescriptor.</summary>
    public class Button : System.Windows.Controls.Button, IDescriptor {
        /// <summary>Identifies the description dependency property.</summary>
        public static readonly DependencyProperty DescriptionProperty;
        /// <summary>Identifies the icon dependency property.</summary>
        public static readonly DependencyProperty IconProperty;
        /// <summary>Identifies the input gesture text dependency property.</summary>
        public static readonly DependencyProperty InputGestureTextProperty;
        /// <summary>Identifies the text dependency property.</summary>
        public static readonly DependencyProperty TextProperty;
        /// <summary>Gets or sets the user-friendly description of the control.</summary>
        public string Description { get => (string)this.GetValue(Button.DescriptionProperty); set => this.SetValue(Button.DescriptionProperty, value); }
        /// <summary>Gets or sets the icon of the control.</summary>
        public object Icon { get => this.GetValue(Button.IconProperty); set => this.SetValue(Button.IconProperty, value); }
        /// <summary>Gets or sets the shortcut text if access key is specified.</summary>
        public string InputGestureText { get => (string)this.GetValue(Button.InputGestureTextProperty); set => this.SetValue(Button.InputGestureTextProperty, value); }
        /// <summary>Gets or sets the text of the control.</summary>
        public string Text { get => (string)this.GetValue(Button.TextProperty); set => this.SetValue(Button.TextProperty, value); }
        static Button() {
            Button.CommandProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(null, Button.OnCommandChanged));
            Button.DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(Button), new FrameworkPropertyMetadata(null));
            Button.IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(Button), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(Button.OnIconChanged)));
            Button.InputGestureTextProperty = DependencyProperty.Register("InputGestureText", typeof(string), typeof(Button), new FrameworkPropertyMetadata(null));
            Button.TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Button), new FrameworkPropertyMetadata(null));
        }
        /// <summary>Constructs a button.</summary>
        public Button() {
        }
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            RelayCommand.RegisterContext(d, e.NewValue);
        }
        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            d.SetValue(Button.ContentProperty, ResourceManager.Get(e.NewValue, out object value) ? value : e.NewValue);
        }
    }
}
