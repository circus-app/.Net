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
// A pivot wrapper around a container of type T that provides filtering.
//
// Same behavior as DataCollection<T> but supporting swapping of source content.
//
// This is intended for collections that execute filtering in an external process
// like a webservice or a database query and swaps its entire content using the
// Filter()/Restore() methods.
//
// Since it is mainly designed to host hierarchical content, the count of elements
// should be provided in the appropriate methods as the class does not parse the
// hierarchy and therefore has no knowledge of children nodes. Moreover, this
// is also to avoid redundant loops since the invoker that performed the query
// already knows the number of items in the source.


#pragma warning disable IDE0002

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
namespace Circus.Wpf.Data {
    /// <summary>Provides a pivot wrapper around a container of type T that provides filtering.</summary>
    public class PivotCollection<T> : ObservableObject, IEnumerable, INotifyCollectionChanged, IPivotCollection  where T : IEnumerable {
        public event PivotFilterCompletedEventHandler FilterCompleted;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public int Count { get => (int)base.GetValue(0); private set => base.SetValue(value); }
        public bool IsFiltered { get => (bool)base.GetValue(false); private set => base.SetValue(value); }
        /// <summary>Returns the source collection.</summary>
        public T Source { get; private set; }
        private PivotCollection() { 
        }
        /// <summary>Constructs a PivotCollection with the specified source collection and the provided number of items.</summary>
        public PivotCollection(T source, int count) {
            this.Source = source;
            this.Count = count;
        }
        private void Dispatch(Action callback) {
            Application.Current.Dispatcher.Invoke(callback);
        }
        /// <summary>Filters the collection using the specified source collection and the provided number of elements.</summary>
        public void Filter(T source, int count) {
            this.Update(source, count, true);
        }
        public virtual IEnumerator GetEnumerator() {
            return this.Source.GetEnumerator();
        }
        /// <summary>The collection changed event handler.</summary>
        public virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (this.CollectionChanged != null) {
                this.Dispatch(() => this.CollectionChanged.Invoke(sender, e));
            }
        }
        private void OnFilterCompleted(bool filtered, bool empty) {
            this.IsFiltered = filtered;
            if (this.FilterCompleted != null) {
                this.Dispatch(() => this.FilterCompleted.Invoke(this, new FilterCompletedEventArgs(filtered, empty)));
            }
        }
        /// <summary>Restores the collection using the specified source collection and the provided number of elements.</summary>
        public void Restore(T source, int count) {
            this.Update(source, count, false);
        }
        private void Update(T source, int count, bool filtered) {
            this.Count = count;
            this.Source = source;
            this.OnFilterCompleted(filtered, count == 0);
            this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
