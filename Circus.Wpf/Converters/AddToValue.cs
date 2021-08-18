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
// A converter to add values between the value property and the value produced
// by the binding source.
//
// Provides a generic implementation of additionning two values if both are 
// numeric and of the same type.
//
// Since binding uses strings for non strong-typed values, it is recommended
// to use value extensions to make sure that the Value property is of valid
// type.
//
// This is mostly used in menu items to enlarge or refine positioning at runtime, 
// according to the value of their related property. See the WindowMenuItem for 
// an example of use.
//
// Converter parameter is ignored.


#pragma warning disable IDE0002

using System;
using System.Globalization;
namespace Circus.Wpf.Converters {
    /// <summary>Provides a converter to add values between the value property and the value produced by the binding source.</summary>
    public sealed class AddToValue : Converter {
        /// <summary>Get or sets the value to add.</summary>
        public object Value { get => base.GetValue(null); set => base.SetValue(value); }
        /// <summary>Constructs a AddToValue converter.</summary>
        public AddToValue() { 
        }
        /// <summary>Constructs a AddToValue converter with the specified value to add.</summary>
        public AddToValue(object value) {
            this.Value = value;
        }
        public override object Convert(object value, Type type, object parameter, CultureInfo culture) {
            return this.Value != null && Numeric.Is(type) && Numeric.Cast(value, type, out dynamic a) && Numeric.Cast(this.Value, type, out dynamic b) ? a + b : base.Convert(value, type, parameter, culture);
        }
        public override object ConvertBack(object value, Type type, object parameter, CultureInfo culture) {
            return this.Value != null && Numeric.Cast(value, type, out dynamic a) && Numeric.Cast(this.Value, type, out dynamic b) ? a - b : base.ConvertBack(value, type, parameter, culture);
        }
    }
}
