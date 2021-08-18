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
// A converter to determine if the provided value is of the specified type.
//
// The class supports types as well as interfaces. Converter parameter is 
// ignored.


#pragma warning disable IDE0002

using System;
using System.Globalization;
namespace Circus.Wpf.Converters {
    /// <summary>Provides a converter to determine if the provided value is of the specified type.</summary>
    public sealed class Is : Converter {
        /// <summary>Gets or sets the targeted type.</summary>
        public Type Type { get => (Type)base.GetValue(null); set => base.SetValue(value); }
        /// <summary>Constructs an Is converter.</summary>
        public Is() { 
        }
        /// <summary>Constructs an Is converter with the specified target type.</summary>
        public Is(Type type) {
            this.Type = type;
        }
        public override object Convert(object value, Type type, object parameter, CultureInfo culture) {
            return Assert.Is(value, this.Type);
        }
    }
}
