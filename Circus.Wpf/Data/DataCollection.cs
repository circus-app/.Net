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
// A wrapper around a container of type T that provides filtering.
//
// The purpose was to provide filtering to any kind of container. Since it is
// mostly intended for binding to an ItemsControl, T should be an observable
// that contains a public Count property. 
//
// The source container must be initialized and filled prior to creating the
// instance. The following example shows a possible implementation using 
// a Basket:
//
// public DataCollection<Basket<string>> Collection { get; set; }
//
// ...
//
// private void Fill() {
//      Basket<string> array = new Basket<string>();
//      ... fill the array ...
//      this.Collection = new DataCollection<Basket<string>>(array);
// }
//
// This applies to flat data only since the predicate will not look for nested 
// collections. Use HierachicalCollection instead if recursivity is required 
// (i.e. TreeView data source, etc.).
//
// The class uses a custom enumerator that matches the specified predicate to
// execute filtering. This avoids adding copies of source items to a filtered
// container and therefore reduces memory usage.
//
// The count property returns the number of items resulting of the current
// enumeration. If not filtered, Count and Source.Count return the same value.
//
// Resetting can be achieved using Reset() or providing a null predicate to
// Filter().
//
// Filtering and resetting use the current application dispatcher to allow
// invocation from other threads.


#pragma warning disable IDE0002

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows;
namespace Circus.Wpf.Data {
    /// <summary>Represents a wrapper around a container of type T that provides filtering.</summary>
    public class DataCollection<T> : DataCollectionBase<T> where T : IEnumerable {
        private Predicate<object> predicate;
        private PropertyInfo property;
        /// <summary>Constructs a data collection using the provided T container.</summary>
        public DataCollection(T source) : base(source) {
            this.predicate = null;
            this.Initialize();
        }
        private IEnumerable Enumerate() {
            foreach (object item in this.Source) {
                if (this.predicate(item)) {
                    base.Count++;
                    yield return item;
                }
            }
        }
        public override void Filter(Predicate<object> predicate) {
            this.predicate = predicate;
            base.IsFiltered = predicate != null;
            Application.Current.Dispatcher.Invoke(() => base.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)));
        }
        public override IEnumerator GetEnumerator() {
            if (this.IsFiltered) {
                base.Count = 0;
                return this.Enumerate().GetEnumerator();
            }
            return base.GetEnumerator();
        }
        private void Initialize() {
            this.property = this.Source.GetType().GetProperty("Count", typeof(int));
            if (base.Source is INotifyCollectionChanged notifiable) {
                CollectionChangedEventManager.AddHandler(notifiable, base.OnCollectionChanged);
            }
            base.Count = this.SourceCount();
        }
        public override void Restore() {
            this.Filter(null);
            base.Count = this.SourceCount();
        }
        private int SourceCount() {
            return this.property != null ? (int)this.property.GetValue(this.Source) : 0;
        }
    }
}
