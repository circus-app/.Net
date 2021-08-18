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


#pragma warning disable IDE0002

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Circus.Collections;
namespace Circus.Wpf.Data {
    internal sealed class HierarchicalCollectionCache : IEnumerable {
        private class Node {
            internal bool Accept { get; set; }
            internal object Item { get; private set; }
            internal int Parent { get; private set; }
            private Node() {
                this.Accept = true;
            }
            internal Node(int parent, object item) : this() {
                this.Parent = parent;
                this.Item = item;
            }
        }
        private readonly Map<int, Node> array;
        private int index;
        internal int Count { get; private set; }
        internal int Excluded => this.array.Count - this.Count;
        internal Predicate<object> Predicate { get; private set; }
        internal HierarchicalCollectionCache() {
            this.array = new Map<int, Node>();
            this.index = 0;
        }
        internal void Filter(IEnumerable source, Predicate<object> predicate) {
            this.Predicate = predicate;
            if (this.array.Empty) {
                this.Register(source);
            }
            this.Update(this.Match());
        }
        private IEnumerable<object> Filtered() {
            foreach (KeyValuePair<int, Node> pair in this.array) {
                if (!pair.Value.Accept) {
                    yield return pair.Value.Item;
                }
            }
        }
        private static bool GetAttribute(Type type, out HierarchicalCollectionAttribute attribute) {
            attribute = type.GetCustomAttribute<HierarchicalCollectionAttribute>(true);
            return attribute != null && !string.IsNullOrEmpty(attribute.Name);
        }
        public IEnumerator GetEnumerator() {
            return this.Filtered().GetEnumerator();
        }
        private int GetParent(object parent) {
            if (parent != null) {
                foreach (KeyValuePair<int, Node> pair in this.array) {
                    if (pair.Value.Item == parent) {
                        return pair.Key;
                    }
                }
            }
            return -1;
        }
        private static bool GetProperty(Type type, string name, out PropertyInfo property) {
            property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return property != null;
        }
        internal static bool IsChildren(object item, out IEnumerable array) {
            Type type = item.GetType();
            array = HierarchicalCollectionCache.GetAttribute(type, out HierarchicalCollectionAttribute attribute) && HierarchicalCollectionCache.GetProperty(type, attribute.Name, out PropertyInfo property) ? (IEnumerable)property.GetValue(item) : null;
            return array != null;
        }
        internal void Invalidate() {
            this.array.Clear(true);
        }
        private Vector<int> Match() {
            Vector<int> array = new Vector<int>();
            foreach (KeyValuePair<int, Node> pair in this.array) {
                bool num = this.Predicate(pair.Value.Item);
                if (num) {
                    array.Add(pair.Key);
                }
                pair.Value.Accept = num;
            }
            this.Count = array.Count;
            return array;
        }
        private void Register(IEnumerable source) {
            foreach (object item in source) {
                this.Register(null, item);
            }
        }
        private void Register(object parent, object item) {
            this.array.Add(this.index++, new Node(this.GetParent(parent), item));
            if (HierarchicalCollectionCache.IsChildren(item, out IEnumerable array)) {
                foreach (object i in array) {
                    this.Register(item, i);
                }
            }
        }
        private void Update(Vector<int> array) {
            foreach (int i in array.End()) {
                int num = this.array[i].Parent;
                while (num != -1 && !this.array[num].Accept) {
                    this.array[num].Accept = true;
                    num = this.array[num].Parent;
                }
            }
        }
    }
}
