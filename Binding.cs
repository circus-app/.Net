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
// A markup extension for custom bindings.
//
// Provides a markup extension to create custom bindings with a ready-to-use 
// implementation of Converter if binding conversion is needed.
//
// Derived types should call Convert in the ProvideValue(IServiceProvider provider) 
// to make sure conversion is applied if specified.


using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
namespace Circus.Wpf {
	/// <summary>Provides a markup extension for custom bindings.</summary>
	[MarkupExtensionReturnType(typeof(object))]
	public abstract class Binding : Markup {
		/// <summary>Returns the converter to use.</summary>
		public IValueConverter Converter { get => (IValueConverter)this.GetValue(null); set => this.SetValue(value); }
		/// <summary>Returns the parameter to pass to the converter.</summary>
		public object ConverterParameter { get => this.GetValue(null); set => this.SetValue(value); }
		/// <summary>Returns the culture in which to evaluate the converter.</summary>
		[TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
		public CultureInfo ConverterCulture { get => base.GetValue(CultureInfo.CurrentCulture); set => this.SetValue(value); }
		/// <summary>Constructs a Binding.</summary>
		protected Binding() {
        }
		/// <summary>Returns the converted value using the converter. If no converter is specified, returns value.</summary>
		protected object Convert(object value) {
			return this.Converter != null ? this.Converter.Convert(value, typeof(object), this.ConverterParameter, this.ConverterCulture) : value;
		}		
    }
}
