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
// A converter to convert an object to string.
//
// Converter parameter is ignored.


using System;
using System.Globalization;
namespace Circus.Wpf.Converters {
    /// <summary>Provides a converter to convert an object to string.</summary>
    public sealed class ToString : Converter {
        /// <summary>Constructs a to string converter.</summary>
        public ToString() { 
        }
        public override object Convert(object value, Type type, object parameter, CultureInfo culture) {
            return value?.ToString();
        }
    }
}
