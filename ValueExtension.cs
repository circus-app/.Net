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
// A generic markup extension for typed values.
//
// Allows to define typed value in markup. This is more convenient than
// the standard synthax <sys:Double>2</sys:Double> ...
//
// The value can be bound "in-line" by specifying its type and value:
//
// Width="{Binding Converter={AddToDouble}, ConverterParameter={Double -2.0}, Path=ActualWidth}"
//
// This is mostly intended to be used in cases where the framework binds to
// an object type, and therefore to avoid parsing values. It is typically
// the case in converters.


using System;
using System.Windows.Markup;
namespace Circus.Wpf {
    /// <summary>Provides a generic markup extension for a typed value.</summary>
    public abstract class ValueExtension<T> : MarkupExtension {
        /// <summary>Gets or sets the value of type T.</summary>
        public T Value { get; set; }
        protected ValueExtension() { 
        }
        protected ValueExtension(T value) {
            this.Value = value;
        }
        public override object ProvideValue(IServiceProvider provider) {
            return this.Value;
        }
    }
}
