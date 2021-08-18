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
// A non-client area window button.


#pragma warning disable IDE0002

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a non-client area window button.</summary>
    public sealed class WindowButton : Button {
        public static readonly DependencyProperty IsActiveProperty;
        [Bindable(true)]
        public bool IsActive { get => (bool)this.GetValue(WindowButton.IsActiveProperty); set => this.SetValue(WindowButton.IsActiveProperty, value); }
        static WindowButton() {
            WindowButton.IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(WindowButton), new FrameworkPropertyMetadata(true));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowButton), new FrameworkPropertyMetadata(typeof(WindowButton)));
        }
        /// <summary>Constructs a window button.</summary>
        public WindowButton() {
        }
    }
}
