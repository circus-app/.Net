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
// A toggle button that implements IDescriptor.
//
// Since icon is added to the content property, caption should be set
// in the text property.


#pragma warning disable IDE0002

using System.Windows;
using Circus.Wpf.Input;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a toggle button that implements IDescriptor.</summary>
    public class ToggleButton : System.Windows.Controls.Primitives.ToggleButton, IDescriptor {
        /// <summary>Identifies the description dependency property.</summary>
        public static readonly DependencyProperty DescriptionProperty;
        /// <summary>Identifies the icon dependency property.</summary>
        public static readonly DependencyProperty IconProperty;
        /// <summary>Identifies the input gesture text dependency property.</summary>
        public static readonly DependencyProperty InputGestureTextProperty;
        /// <summary>Identifies the text dependency property.</summary>
        public static readonly DependencyProperty TextProperty;
        /// <summary>Gets or sets the user-friendly description of the control.</summary>
        public string Description { get => (string)this.GetValue(ToggleButton.DescriptionProperty); set => this.SetValue(ToggleButton.DescriptionProperty, value); }
        /// <summary>Gets or sets the icon of the control.</summary>
        public object Icon { get => this.GetValue(ToggleButton.IconProperty); set => this.SetValue(ToggleButton.IconProperty, value); }
        /// <summary>Gets or sets the shortcut text if access key is specified.</summary>
        public string InputGestureText { get => (string)this.GetValue(ToggleButton.InputGestureTextProperty); set => this.SetValue(ToggleButton.InputGestureTextProperty, value); }
        /// <summary>Gets or sets the text of the control.</summary>
        public string Text { get => (string)this.GetValue(ToggleButton.TextProperty); set => this.SetValue(ToggleButton.TextProperty, value); }
        static ToggleButton() {
            ToggleButton.CommandProperty.OverrideMetadata(typeof(ToggleButton), new FrameworkPropertyMetadata(null, ToggleButton.OnCommandChanged));
            ToggleButton.DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(ToggleButton), new FrameworkPropertyMetadata(null));
            ToggleButton.IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(ToggleButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ToggleButton.OnIconChanged)));
            ToggleButton.InputGestureTextProperty = DependencyProperty.Register("InputGestureText", typeof(string), typeof(ToggleButton), new FrameworkPropertyMetadata(null));
            ToggleButton.TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ToggleButton), new FrameworkPropertyMetadata(null));
        }
        /// <summary>Constructs a toggle button.</summary>
        public ToggleButton() {
        }
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            RelayCommand.RegisterContext(d, e.NewValue);
        }
        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            d.SetValue(ToggleButton.ContentProperty, ResourceManager.Get(e.NewValue, out object value) ? value : e.NewValue);
        }
    }
}
