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
// A component to cache TreeViewItems states.
//
// The entire hierarchy of a TreeViewBase is cached in a map that uses either the 
// item hash code or, if available, the item data context hash code that serves
// as a unique identifier of a particular node. In case of data context binding,
// the source object should override the GetHashCode() method and return an
// immutable value to ensure reference consistency (i.e. do not use the text
// property of a tree node since it may change if the node is renamed, etc.).
//
// Note that for PivotTreeView data source, it is highly recommended to use
// a database unique key due to base collection view being reset during 
// filtering and therefore losing all instance references to the cached content.
//
// The class provides two methods of restoring items states:
//
// - Using the Collapsed() enumeration to collapse the nodes that have been
//   expanded during filtering. This only applies to TreeView.
// - Using the Restore() method that reconnects disconnected items during
//   filtering as it happens in PivotTreeView.


#pragma warning disable IDE0002

using System.Collections.Generic;
using Circus.Collections;
namespace Circus.Wpf.Controls {
    internal class TreeViewSource : ITreeViewSource {
        private class Entry { 
            internal bool IsExpanded { get; set; }
            internal TreeViewItem Item { get; set; }
            internal Entry(TreeViewItem item) {
                this.Item = item;
                this.IsExpanded = item.IsExpanded;
            }
        }
        private Map<int, Entry> array;
        public int Count => this.array.Count;
        private TreeViewSource() {
        }
        internal TreeViewSource(int count) {
            this.array = new Map<int, Entry>(count < 5 ? 5 : count);
        }
        public bool Add(TreeViewItem item) {
            return this.array.Add(this.Key(item), new Entry(item));
        }
        public IEnumerable<TreeViewItem> Collapsed() {
            foreach (Entry e in this.array.Values()) {
                if (!e.IsExpanded) {
                    yield return e.Item;
                }
            }
        }
        public virtual void Dispose() {
            this.array.Clear(true);
            this.array = null;
        }
        internal bool Get(object key, out TreeViewItem item) {
            return this.Get(key.GetHashCode(), out item);
        }
        internal bool Get(int value, out TreeViewItem item) {
            bool num = this.array.Get(value, out Entry entry);
            item = num ? entry.Item : null;
            return item != null;
        }
        private int Key(TreeViewItem item) {
            return item.DataContext != null ? item.DataContext.GetHashCode() : item.GetHashCode();
        }
        public virtual bool Remove(int value) {
            return this.array.Remove(value);
        }
        public bool Restore(TreeViewItem item) {
            if (this.array.Get(this.Key(item), out Entry value)) {
                value.Item = item;
                item.IsExpanded = value.IsExpanded;
                return true;
            }
            return false;
        }
        public void Toggle(TreeViewItem item) {
            this.array[this.Key(item)].IsExpanded = item.IsExpanded;
        }
    }
}
