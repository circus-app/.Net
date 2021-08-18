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
// A markup extension to define sizing behavior.
//
// It provides a sizing mode (auto/fixed) and a value to determine how an item
// should be resized.
//
// Since value is just a double, the consumer is responsible for implementing 
// the desired behavior (height, width, etc.).
//
// It is used in cases where an item can either be auto-sized or fixed-sized 
// depending on criterias. See SideMenu for an example of implementation.


#pragma warning disable IDE0002

using System;
using System.Windows.Markup;
namespace Circus.Wpf {
    /// <summary>Provides a markup extension to define sizing behavior.</summary>
    [MarkupExtensionReturnType(typeof(SizeMode))]
    public sealed class SizeMode : Binding {
        /// <summary>Gets or sets the mode of resizing.</summary>
        public SizeModes Mode { get => (SizeModes)base.GetValue(SizeModes.Auto); set => base.SetValue(value); }
        /// <summary>Gets or sets the value to use when resizing.</summary>
        public double Value { get => (double)base.GetValue(0.0); set => base.SetValue(value); }
        /// <summary>Constructs a size mode with the default mode and value.</summary>
        public SizeMode() { 
        }
        /// <summary>Constructs a size mode with the default mode and the specified value.</summary>
        public SizeMode(double value) {
            this.Value = value;
        }
        /// <summary>Constructs a size mode with the specified mode and value.</summary>
        public SizeMode(SizeModes mode, double value) {
            this.Mode = mode;
            this.Value = value;
        }
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
