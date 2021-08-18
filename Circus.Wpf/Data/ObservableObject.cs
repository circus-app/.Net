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
// A base class for an object that reports property changes.
//
// Inline initialization of properties must implement GetValue()/SetValue(),
// providing a default value in getter: 
//
// public object SomeProperty { 
//		get => (object)base.GetValue(new object());
//		set => base.SetValue(value); 
// }


#pragma warning disable IDE0002

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Circus.Collections;
namespace Circus.Wpf.Data {
	/// <summary>Provides a base class for an object that reports property changes.</summary>
	[Obsolete("Use Circus.Model.ObservableObject instead")]
	public abstract class ObservableObject : INotifyPropertyChanged {
        private readonly Map<string, object> array;
        public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>Constructs an observable object.</summary>
		protected ObservableObject() {
			this.array = new Map<string, object>();
		}
		private static bool Assert(string value) {
			return value.Length > 0;
		}
		/// <summary>Returns the value associated to the specified name or value if the key does not exists.</summary>
		protected object GetValue(object value, [CallerMemberName] string name = "") {
			if (!ObservableObject.Assert(name) || !this.array.Get(name, out object result)) {
				return value;
			}
			return result;
		}
		/// <summary>Sets the value associated to the specified name.</summary>
		protected void SetValue(object value, [CallerMemberName] string name = "") {
			if (!ObservableObject.Assert(name) || (this.array.Get(name, out object result) && result.Equals(value))) {
				return;
			}
			this.array.AddOrUpdate(name, value);
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
