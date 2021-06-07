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
// A manager of property changed events for an INotifyPropertyChanged object that
// belongs to a data source.
//
// Because a data source object is intended to be bound to a control that 
// potentially allows the end user to modify values (i.e. rename a tree node, 
// change a list or a data grid cell value, etc.), it is useful to be notified of 
// these changes in case of further action is required (i.e. update a database, 
// save to file, etc.).
//
// The property changed manager is here to provide an handy way of achieving such
// task by registering an INotifyPropertyChanged object to the native
// PropertyChangedEventManager using the specified DataSource object as target.
//
// The component searches for a PropertyChangeAttribute on the provided object
// and attaches the data source OnItemPropertyChanged event handler to each
// properties defined. Note that the object must implement INotifyPropertyChanged.


#pragma warning disable IDE0002

using System;
using System.ComponentModel;
using Circus.Runtime;
namespace Circus.Model {
    /// <summary>Provides a manager of property changed events for an INotifyPropertyChanged object that belongs to a data source.</summary>
    public sealed class PropertyChangedManager {
        private PropertyChangedManager() {
        }
        /// <summary>Adds the specified property changed event handler for properties of the provided object defined in its PropertyChangeAttribute.</summary>
        public static void AddHandler(object obj, EventHandler<PropertyChangedEventArgs> handler) {
            if (Traits.GetAttribute(obj, out PropertyChangedAttribute attribute) && Assert.As(obj, out INotifyPropertyChanged notify)) {
                foreach (string name in attribute.Values) {
                    PropertyChangedEventManager.AddHandler(notify, handler, name);
                }
            }
        }
        /// <summary>Removes the specified property changed event handler for properties of the provided object defined in its PropertyChangeAttribute.</summary>
        public static void RemoveHandler(object obj, EventHandler<PropertyChangedEventArgs> handler) {
            if (Traits.GetAttribute(obj, out PropertyChangedAttribute attribute) && Assert.As(obj, out INotifyPropertyChanged notify)) {
                foreach (string name in attribute.Values) {
                    PropertyChangedEventManager.RemoveHandler(notify, handler, name);
                }
            }
        }
    }
}
