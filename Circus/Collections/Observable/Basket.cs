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
// An observable vector of T elements.
//
// Same implementation as Vector<T>.


#pragma warning disable IDE0002

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
namespace Circus.Collections.Observable {
    /// <summary>Provides an observable vector of T elements.</summary>
    [Serializable]
    public class Basket<T> : Vector<T>, IObservable {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        public override T this[int index] {
            get {
                return base[index];
            }
            set {
                base[index] = value;
                this.OnCollectionChanged(NotifyCollectionChangedAction.Add, value, index, false);
            }
        }
        /// <summary>Constructs an observable vector with the default capacity and offset.</summary>
        public Basket() : base() {
        }
        /// <summary>Constructs an observable vector with the specified capacity and the default offset.</summary>
        public Basket(int capacity) : base(capacity) {
        }
        public Basket(T fill) : base(fill) {
        }
        /// <summary>Constructs an observable vector with the specified capacity and offset.</summary>
        public Basket(int capacity, int offset) : base(capacity, offset) {
        }
        /// <summary>Constructs an observable vector with the specified capacity and the default offset. Each element is a copy of the fill value.</summary>
        public Basket(int capacity, T fill) : base(capacity, fill) {
        }
        /// <summary>Constructs an observable vector with the specified capacity and offset. Each element is a copy of the fill value.</summary>
        public Basket(int capacity, int offset, T fill) : base(capacity, offset, fill) {
        }
        /// <summary>Constructs an observable vector with a copy of each of the elements in array, in the same order.</summary>
        public Basket(IEnumerable<T> array) : base(array) {
        }
        /// <summary>Constructs an observable vector with the specified offset and with a copy of each of the elements in array, in the same order.</summary>
        public Basket(int offset, IEnumerable<T> array) : base(offset, array) {
        }
        public override int Add(T value) {
            int num = base.Add(value);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, value, num, true);
            return num;
        }
        public override void Clear() {
            base.Clear();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Reset, null, -1, true);
        }
        public override void Clear(bool trim) {
            base.Clear(trim);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Reset, null, -1, true);
        }
        public override void CopyTo(Array array, int index) {
            base.CopyTo(array, index);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, array, index, true);
        }
        private IEnumerable<T> GetRange(int index, int count) {
            Vector<T> result = new Vector<T>(count);
            for (int i = index; i < index + count; i++) {
                result.Add(base[i]);
            }
            return result;
        }
        public override void Insert(int index, IEnumerable<T> array) {
            base.Insert(index, array);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, array, index, true);
        }
        public override void Insert(int index, T value) {
            base.Insert(index, value);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, value, index, true);
        }
        public override bool InsertAt(int index, IEnumerable<T> array) {
            return base.InsertAt(index, array) && this.OnCollectionChanged(NotifyCollectionChangedAction.Add, array, index, true);
        }
        public override bool InsertAt(int index, T value) {
            return base.InsertAt(index, value) && this.OnCollectionChanged(NotifyCollectionChangedAction.Add, value, index, true);
        }
        /// <summary>Raises the collection changed event with the provided arguments.</summary>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
            this.CollectionChanged?.Invoke(this, e);
        }
        private bool OnCollectionChanged(NotifyCollectionChangedAction action, object value, int index, bool update) {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, value, index));
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
        public override void Remove(int index) {
            T value = base[index];
            base.Remove(index);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, value, index, true);
        }
        public override void Remove(int index, int count) {
            IEnumerable<T> array = this.GetRange(index, count);
            base.Remove(index, count);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, array, index, true);
        }
        public override bool RemoveAt(int index) {
            T value = base[index];
            return base.RemoveAt(index) && this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, value, index, true);
        }
        public override bool RemoveAt(int index, int count) {
            IEnumerable<T> array = this.GetRange(index, count);
            return base.RemoveAt(index, count) && this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, array, index, true);
        }
        public override void Resize(int size) {
            bool num = size < base.Size;
            base.Resize(size);
            if (num) {
                this.OnCollectionChanged(NotifyCollectionChangedAction.Reset, null, -1, true);
            }
        }
        public override void Reverse() {
            base.Reverse();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Move, null, 0, false);
        }
        public override void Reverse(int index, int count) {
            IEnumerable<T> array = this.GetRange(index, count);
            base.Reverse(index, count);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Move, array, index, false);
        }
        public override bool ReverseAt(int index, int count) {
            IEnumerable<T> array = this.GetRange(index, count);
            return base.ReverseAt(index, count) && this.OnCollectionChanged(NotifyCollectionChangedAction.Move, array, index, false);
        }
        public override void Swap(Vector<T> vector) {
            base.Swap(vector);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Reset, null, 0, true);
        }
    }
}
