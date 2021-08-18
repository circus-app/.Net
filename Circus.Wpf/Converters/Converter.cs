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
// A markup extension for converters.


using System;
using System.Globalization;
using System.Windows.Data;
namespace Circus.Wpf.Converters {
    /// <summary>Provides a markup extension for a converter.</summary>
    public abstract class Converter : Markup, IValueConverter {
        /// <summary>Constructs a converter.</summary>
        protected Converter() { 
        }
        public virtual object Convert(object value, Type type, object parameter, CultureInfo culture) {
            return value;
        }
        public virtual object ConvertBack(object value, Type type, object parameter, CultureInfo culture) {
            return value;
        }
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
