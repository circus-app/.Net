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
// A boolean to visibility converter.
//
// The converter can optionally set the invert property to negate the value 
// before conversion. This is useful in cases where visibility is the inverse 
// of an inherited UIElement state. 
//
// Static methods are here to provide a unified conversion strategy. Unless
// specific cases and for consistency, code that needs to perform conversion 
// should call the static members instead of writing its own implementation.
//
// Converter parameter is ignored.


#pragma warning disable IDE0002

using System;
using System.Globalization;
using System.Windows;
namespace Circus.Wpf.Converters {
    /// <summary>Provides a boolean to visibility converter.</summary>
    public sealed class BooleanToVisibility : Converter {
        /// <summary>Determines if value must be inverted before conversion.</summary>
        public bool Invert { get => (bool)base.GetValue(false); set => base.SetValue(value); }
        /// <summary>Constructs a boolean to visibility converter.</summary>
        public BooleanToVisibility() {
        }
        /// <summary>Constructs a boolean to visibility converter with the specified invert flag.</summary>
        public BooleanToVisibility(bool invert) {
            this.Invert = invert;
        }
        /// <summary>Converts the specified boolean to visibility.</summary>
        public static Visibility Convert(bool value) {
            return BooleanToVisibility.Convert(false, value);
        }
        /// <summary>Converts the specified boolean to visibility using the provided inverting option.</summary>
        public static Visibility Convert(bool invert, bool value) {
            if (invert) {
                value = !value;
            }
            return value ? Visibility.Visible : Visibility.Collapsed;
        }
        public override object Convert(object value, Type type, object parameter, CultureInfo culture) {
            return value is bool boolean && type == typeof(Visibility) ? BooleanToVisibility.Convert(this.Invert, boolean) : base.Convert(value, type, parameter, culture);
        }
        /// <summary>Converts the specified visibility to a boolean.</summary>
        public static bool ConvertBack(Visibility value) {
            return value == Visibility.Visible;
        }
        public override object ConvertBack(object value, Type type, object parameter, CultureInfo culture) {
            return value is Visibility visibility && type == typeof(bool) ? BooleanToVisibility.ConvertBack(visibility) : base.ConvertBack(value, type, parameter, culture);
        }
    }
}
