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
// A hierarchical wrapper around a container of type T that provides filtering.
//
// Same behavior as DataCollection<T> but supporting nested containers.
//
// This is a more complex scenario. Let's consider this simple class as our
// data object bound to our hierarchical control (i.e. TreeView):
//
// [HierarchicalCollection("Items")]
// public class MyClass() {      
//      public Basket<MyClass> Items { get; set; }
//      public string Name { get; set; }
//      public MyClass() {
//      }
// }
//
// The above example can be nested endlessly since it provides a container of
// its own type (aka Items). In that case, filtering needs to walk down the 
// entire hierarchy to figure out which items are eligible but then also update
// any parent node that wouldn't match the predicate but contains valid items.
//
// It is achieved using a HierarchicalCollectionCache that projects the entire 
// object hierarchy into a map containing a reference to the object itself and 
// a pointer to its parent represented by an internal index.
//
// Children are determined by the HierarchicalCollection attribute. This
// allows to specifically target the property that contains the descendant 
// nodes and therefore provides a full control over the hierarchy.
//
// The first pass determines which node matches the predicate and stores
// its index in an internal vector. The second pass iterates the vector
// and walks up each ancestor recursively updating its state (if it was not
// matched in the first pass).
//
// Finally, the cache raises the FilterCompleted event and returns an enumerator
// containing the excluded items.
//
// FilterCompleted is invoked in the application main thread since Filter method
// can be called from an asynchonous member (i.e. DataSource.Filter) and therefore
// allows bound controls to use their default dispatcher. 
//
// This implementation avoids creating a copy of the entire data source since 
// we only provide pointers to the items that should be excluded.
//
// The cache is entirely recreated whenever the source changes.
//
// Because each container has its own CollectionChanged event handler, we need
// to be notified of any change at any level to determine if our cache requires 
// invalidation. This is done by hooking the CollectionChanged event for each 
// node and its children to the class event handler.


#pragma warning disable IDE0002

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
namespace Circus.Wpf.Data {
    /// <summary>Represents a hierarchical wrapper around a container of type T that provides filtering.</summary>
    public class HierarchicalCollection<T> : DataCollectionBase<T>, IHierarchicalCollection where T : IEnumerable {
        private readonly HierarchicalCollectionCache cache;
        private int count;
        public event HierarchicalFilterCompletedEventHandler FilterCompleted;
        /// <summary>Constructs a hierarchical collection using the provided T container.</summary>
        public HierarchicalCollection(T source) : base(source) {
            this.cache = new HierarchicalCollectionCache();
            this.count = 0;
            this.Initialize();
            this.Register();
        }
        public override void Filter(Predicate<object> predicate) {
            this.cache.Filter(this.Source, predicate);
            this.Update(this.cache.Count);
            this.OnFilterCompleted(this.cache.Excluded, true, this.cache.Count == 0);
        }
        public override IEnumerator GetEnumerator() {
            return this.IsFiltered ? this.cache.GetEnumerator() : base.GetEnumerator();
        }
        private void HookCollectionChanged(object source, bool flag) {
            if (source is INotifyCollectionChanged collection) {
                if (flag) {
                    CollectionChangedEventManager.AddHandler(collection, this.OnCollectionChanged);
                    return;
                }
                CollectionChangedEventManager.RemoveHandler(collection, this.OnCollectionChanged);
            }
        }
        private void Initialize() {
            base.IsFiltered = false;
            this.HookCollectionChanged(base.Source, true);
        }
        public override void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add: this.Register(e.NewItems, true); break;
                case NotifyCollectionChangedAction.Remove: this.Register(e.OldItems, false); break;
                case NotifyCollectionChangedAction.Replace: this.Register(e.OldItems, false); this.Register(e.NewItems, true); break;
            }
            this.cache.Invalidate();
            if (sender.Equals(this.Source)) {
                base.OnCollectionChanged(sender, e);
            }
        }
        private void OnFilterCompleted(int count, bool filtered, bool empty) {
            base.IsFiltered = filtered;
            Application.Current.Dispatcher.Invoke(() => this.FilterCompleted?.Invoke(this, new HierarchicalFilterCompletedEventArgs(count, filtered, empty)));
        }
        private void Register() {
            foreach (object item in this.Source) {
                this.Register(item, true);
            }
            this.Update(this.count);
        }
        private void Register(IList array, bool flag) {
            foreach (object item in array) {
                this.Register(item, flag);
            }
        }
        private void Register(object item, bool flag) {
            this.count += flag ? 1 : -1;
            if (HierarchicalCollectionCache.IsChildren(item, out IEnumerable array)) {
                this.HookCollectionChanged(array, flag);
                foreach (object i in array) {
                    this.Register(i, flag);
                }
            }
        }
        public override void Restore() {
            this.Update(this.count);
            this.OnFilterCompleted(0, false, false);
        }
        private void Update(int value) {
            base.Count = value;
        }
    }
}
