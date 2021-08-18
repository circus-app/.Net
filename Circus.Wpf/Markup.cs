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
// A base class for markup extension.
//
// Inline initialization of properties must implement GetValue()/SetValue(),
// providing a default value in getter: 
//
// public object SomeProperty { 
//		get => base.GetValue<Foo>(new Foo());
//		set => base.SetValue(value); 
// }


#pragma warning disable IDE0002

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using Circus.Collections;
namespace Circus.Wpf {
	/// <summary>Provides a base class for markup extension with inline initialization of properties.</summary>
	[MarkupExtensionReturnType(typeof(object))]
	public abstract class Markup : MarkupExtension, INotifyPropertyChanged {
        private readonly Map<string, object> array;
		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>Constructs a markup.</summary>
		protected Markup() {
			this.array = new Map<string, object>();
		}
        /// <summary>Returns the value associated to the specified name or value if the key does not exists.</summary>
        protected object GetValue(object value, [CallerMemberName] string name = "") {
			return this.array.Get(name, out object result) ? result : value;
		}
		/// <summary>Returns the T value associated to the specified name or value if the key does not exists.</summary>
		protected T GetValue<T>(T value, [CallerMemberName] string name = "") {
			return (T)this.GetValue((object)value, name);
		}
		/// <summary>Sets the value associated to the specified name.</summary>
		protected void SetValue(object value, [CallerMemberName] string name = "") {
			if (this.array.Get(name, out object result) && result == value) {
				return;
			}
			this.array.AddOrUpdate(name, value);
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
