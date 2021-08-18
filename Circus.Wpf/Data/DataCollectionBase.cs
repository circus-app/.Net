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
// A base class for a data collection.


#pragma warning disable IDE0002

using System;
using System.Collections;
using System.Collections.Specialized;
namespace Circus.Wpf.Data {
    /// <summary>Provides a base class for a data collection.</summary>
    public abstract class DataCollectionBase<T> : ObservableObject, IDataCollection, IEnumerable, INotifyCollectionChanged where T : IEnumerable {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public int Count { get => (int)base.GetValue(0); protected set => base.SetValue(value); }
        public bool IsFiltered { get => (bool)base.GetValue(false); protected set => base.SetValue(value); }
        /// <summary>Returns the source collection.</summary>
        public T Source { get; protected set; }
        private DataCollectionBase() {
        }
        protected DataCollectionBase(T Source) {
            this.Source = Source;
        }
        /// <summary>Filters the collection using the specified predicate.</summary>
        public virtual void Filter(Predicate<object> predicate) { 
        }
        public virtual IEnumerator GetEnumerator() {
            return this.Source.GetEnumerator();
        }
        /// <summary>The collection changed event handler.</summary>
        public virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            this.CollectionChanged?.Invoke(sender, e);
        }
        /// <summary>Restores the collection to its original content.</summary>
        public virtual void Restore() { 
        }
    }
}
