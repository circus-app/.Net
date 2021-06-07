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
// An observable set of T elements.
//
// Same implementation as Set<T>.


#pragma warning disable IDE0002

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
namespace Circus.Collections.Observable {
    /// <summary>Provides an observable set of T elements.</summary>
    [Serializable]
    public class ObservableSet<T> : Set<T>, IObservable {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>Constructs a container with the default capacity and offset.</summary>
        public ObservableSet() : base(5, 5) {
        }
        /// <summary>Constructs a container with the specified capacity and the default offset.</summary>
        public ObservableSet(int capacity) : base(capacity, 5) {
        }
        /// <summary>Constructs a container with a copy of each of the elements in array.</summary>
        public ObservableSet(IEnumerable<T> array) : base(5, array) {
        }
        /// <summary>Constructs a container with the specified equality comparer.</summary>
        public ObservableSet(IEqualityComparer<T> comparer) : base(5, 5, comparer) {
        }
        /// <summary>Constructs a container with the specifed capacity and offset.</summary>
        public ObservableSet(int capacity, int offset) : base(capacity, offset, EqualityComparer<T>.Default) {
        }
        /// <summary>Constructs a container with the specified offset and a copy of each of the elements in array.</summary>
        public ObservableSet(int offset, IEnumerable<T> array) : base(offset, array, EqualityComparer<T>.Default) {
        }
        /// <summary>Constructs a container with the specified capacity and equality comparer.</summary>
        public ObservableSet(int capacity, IEqualityComparer<T> comparer) : base(capacity, 5, comparer) {
        }
        /// <summary>Constructs a container with a copy of each of the elements in array and the specified equality comparer.</summary>
        public ObservableSet(IEnumerable<T> array, IEqualityComparer<T> comparer) : base(5, array, comparer) {
        }
        /// <summary>Constructs a container with the specified capacity, offset and equality comparer.</summary>
        public ObservableSet(int capacity, int offset, IEqualityComparer<T> comparer) : base(capacity, offset, comparer) {
        }
        /// <summary>Constructs a container with the specified offset, a copy of each of the elements in array and the specified equality comparer.</summary>
        public ObservableSet(int offset, IEnumerable<T> array, IEqualityComparer<T> comparer) : base(offset, array, comparer) {
        }
        public override bool Add(T value) {
            bool num = base.Add(value);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, value, true);
            return num;
        }
        public override void Clear() {
            base.Clear();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Reset, null, true);
        }
        public override void Clear(bool trim) {
            base.Clear(trim);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Reset, null, true);
        }
        /// <summary>Raises the collection changed event with the provided arguments.</summary>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
            this.CollectionChanged?.Invoke(this, e);
        }
        private bool OnCollectionChanged(NotifyCollectionChangedAction action, object value, bool update) {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, value, 0));
            if (update) {
                this.OnPropertyChanged("Count");
            }
            return true;
        }
        /// <summary>Raises the property changed event with the provided arguments.</summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
            this.PropertyChanged?.Invoke(this, e);
        }
        private void OnPropertyChanged(string name) {
            this.OnPropertyChanged(new PropertyChangedEventArgs(name));
        }
        public override bool Remove(T value) {
            bool num = base.Remove(value);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, value, true);
            return num;
        }
        public override void Resize(int size) {
            bool num = size < base.Size;
            base.Resize(size);
            if (num) {
                this.OnCollectionChanged(NotifyCollectionChangedAction.Reset, null, true);
            }
        }
        public override void Swap(Set<T> set) {
            base.Swap(set);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Reset, null, true);
        }
    }
}
