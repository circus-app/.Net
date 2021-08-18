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
// A lightweight control for displaying an icon and a text.
//
// Since icon is added to the content property, caption should be set in the 
// text property.


#pragma warning disable IDE0002

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a lightweight control for displaying an icon and a text.</summary>
    public class Label : ContentControl {
        /// <summary>Identifies the icon dependency property.</summary>
        public static readonly DependencyProperty IconProperty;
        /// <summary>Identifies the icon placement dependency property.</summary>
        public static readonly DependencyProperty IconPlacementProperty;
        /// <summary>Identifies the text dependency property.</summary>
        public static readonly DependencyProperty TextProperty;
        /// <summary>Identifies the text trimming dependency property.</summary>
        public static readonly DependencyProperty TextTrimmingProperty;
        /// <summary>Gets or sets the icon of the control.</summary>
        [Bindable(true)]
        public object Icon { get => this.GetValue(Label.IconProperty); set => this.SetValue(Label.IconProperty, value); }
        /// <summary>Gets or sets the margin applied between the icon and the text.</summary>
        public Thickness IconPlacement { get => (Thickness)this.GetValue(Label.IconPlacementProperty); set => this.SetValue(Label.IconPlacementProperty, value); }
        /// <summary>Gets or sets the text of the control.</summary>
        [Bindable(true)]
        public string Text { get => (string)this.GetValue(Label.TextProperty); set => this.SetValue(Label.TextProperty, value); }
        /// <summary>Gets or sets the text trimming behavior to employ when text overflows the text area.</summary>
        [Bindable(true)]
        public TextTrimming TextTrimming { get => (TextTrimming)this.GetValue(Label.TextTrimmingProperty); set => this.SetValue(Label.TextTrimmingProperty, value); }
        static Label() {
            Label.IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(Label), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(Label.OnIconChanged)));
            Label.IconPlacementProperty = DependencyProperty.Register("IconPlacement", typeof(Thickness), typeof(Label), new FrameworkPropertyMetadata(new Thickness(0)));
            Label.TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Label), new FrameworkPropertyMetadata(null));
            Label.TextTrimmingProperty = DependencyProperty.Register("TextTrimming", typeof(TextTrimming), typeof(Label), new FrameworkPropertyMetadata(TextTrimming.None));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Label), new FrameworkPropertyMetadata(typeof(Label)));
        }
        /// <summary>Constructs a label.</summary>
        public Label() {
        }
        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            d.SetValue(Label.ContentProperty, ResourceManager.Get(e.NewValue, out object value) ? value : e.NewValue);
        }
    }
}
