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
// A converter to determine if the provided value is null.
//
// Objects and strings are supported. Trim optionally removes leading and 
// trailing white spaces from value before checking its length, which ends 
// up being the same as a string.IsNullOrWhitespace().
//
// Trim is optional for cases where white spaces are valid values. It is 
// ignored if the value is not of type string. 


#pragma warning disable IDE0002

using System;
using System.Globalization;
namespace Circus.Wpf.Converters {
    /// <summary>Provides a converter to determine if the provided value is null.</summary>
    public sealed class IsNull : Converter {
        /// <summary>Determines if the value should be trimmed prior to checking. Only applies to string type.</summary>
        public bool Trim { get => (bool)base.GetValue(false); set => base.SetValue(value); }
        /// <summary>Construct an is null converter.</summary>
        public IsNull() { 
        }
        /// <summary>Construct an is null converter with the specified trimming option.</summary>
        public IsNull(bool trim) {
            this.Trim = trim;
        }
        public override object Convert(object value, Type type, object parameter, CultureInfo culture) {
            if (value is string) {
                string s = value.ToString();
                return string.IsNullOrEmpty(s) || (this.Trim && s.Trim().Length == 0);
            }
            return value == null;
        }
    }
}
