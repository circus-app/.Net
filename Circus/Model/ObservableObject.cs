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
// An object that reports property changes.


#pragma warning disable IDE0002

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Circus.Collections;
namespace Circus.Model {
	/// <summary>Provides an object that reports property changes.</summary>
	public abstract class ObservableObject : INotifyPropertyChanged {
		private readonly Map<string, object> array;
		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>Constructs an ObservableObject.</summary>
		protected ObservableObject() {
			this.array = new Map<string, object>();
		}
		/// <summary>Returns the value of a property on this instance of an ObservableObject or the specified default value if the property is unset.</summary>
		protected object GetValue(object value, [CallerMemberName] string name = "") {
			return this.array.Get(name, out object result) ? result : value;
		}
		/// <summary>Returns the T value associated to the specified name or value if the key does not exists.</summary>
		protected T GetValue<T>(T value, [CallerMemberName] string name = "") {
			return (T)this.GetValue((object)value, name);
		}
		/// <summary>Sets the value of a property on this instance of an ObservableObject.</summary>
		protected void SetValue(object value, [CallerMemberName] string name = "") {
			if (this.array.Get(name, out object result) && result.Equals(value)) {
				return;
			}
			this.array.AddOrUpdate(name, value);
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
