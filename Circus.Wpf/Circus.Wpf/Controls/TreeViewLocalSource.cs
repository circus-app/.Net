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

using System.Collections.Generic;
using Circus.Collections;
namespace Circus.Wpf.Controls {
    internal class TreeViewLocalSource : TreeViewSource {
        private Vector<Set<int>> array;
        private int index;
        internal TreeViewLocalSource(int count) : base(count) {
            this.index = 0;
        }
        private bool All(int value, out TreeViewItem item) {
            return base.Get(value, out item);
        }
        private void Assert() {
            if (this.array == null) {
                this.array = new Vector<Set<int>>(2);
            }
        }
        public override void Dispose() {
            if (this.array != null) {
                foreach (Set<int> set in this.array) {
                    set.Clear(true);
                }
                this.array.Clear(true);
                this.array = null;
            }
            base.Dispose();
        }
        private bool Distinct(int value, out TreeViewItem item) {
            bool num = !this.array[1].Contains(value);
            item = num && base.Get(value, out TreeViewItem i) ? i : null;
            return num;
        }
        internal bool Pop(bool clear, out IEnumerable<TreeViewItem> array) {
            array = this.index == 0 && !clear ? null : Pail<TreeViewItem>.Create(this.array[0], clear ? (Pail<TreeViewItem>.Predicate<int>)this.All : this.Distinct);
            bool num = array != null;
            if (num) {
                this.array.Remove(0);
            }
            return num;
        }
        internal bool Push(object key, out TreeViewItem item) {
            bool num = base.Get(key, out item);
            return num && this.array[this.index].Add(key.GetHashCode());
        }
        public override bool Remove(int value) {
            if (!this.array.Empty) {
                this.array[0].Remove(value);
            }
            return base.Remove(value);
        }
        internal void Reserve(int count) {
            this.Assert();
            this.index = this.array.Add(new Set<int>(count));
        }
    }
}