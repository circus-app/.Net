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
// A dynamic contiguous thread-safe container of T elements. 
//
// Same implementation as Vector<T>.
//
// Thread-safety is ensured by atomic read/writes when possible which makes Bag
// a lock-free container for insertion, accessors and iterators.
//
// Concurrency defines the number of threads accessing the container. The higher
// value, the more concurrent writes can take place without blocking. Default 
// value is the number of CPUs. Higher concurrency values cause operations that 
// require all locks like Count, Size, Insert(), Remove(), Reserve(), Resize(),
// Reverse() and Trim() to become more expensive.
//
// Each element is encapsulated in an Entry class that is a wrapper around T. 
// Because it is a reference type, it allows volatile read/writes and therefore 
// avoids constraining T to class() (which would forbid bags of value types).
//
// Elements are stored in a contiguous array to preserve random access constant 
// O(1). 
//
// Methods with Core extension do not acquire locks to avoid double locking since
// they are called from multiple points within the class. Locking is ensured by 
// invokers when it is required.
//
// Initialization or insertion from a non Bag enumerable has a significant 
// performance overhead since elements must be wrapped in an Entry instance within 
// a loop.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
namespace Circus.Collections.Concurrency {
    /// <summary>Provides a contiguous thread-safe container of T elements. Default capacity and offset is 5.</summary>
    [Serializable]
    public class Bag<T> : IVector<T>, IEnumerable<T> {
        private int count;
        private volatile Threads<Entry<T>> list;
        public T this[int index] { get => this.Get(index); set => this.Update(index, value); }
        public T Back { get => this.Get(count > 0 ? count - 1 : 0); set => this.Update(count > 0 ? count - 1 : 0, value); } 
        public int Capacity { get; private set; }
        public int Count { 
            get {
                int num = list.Freeze();
                try {
                    return count;
                }
                finally {
                    list.Unfreeze(num);
                }
            } 
        }
        public bool Empty => this.Count == 0;
        public T Front { get => this.Get(0); set => this.Update(0, value); }
        public int Offset { get; private set; }
        public int Size {
            get {
                int num = list.Freeze();
                try {
                    return list.Entries.Length;
                }
                finally {
                    list.Unfreeze(num);
                }
            }
        }
        /// <summary>Constructs a container with the default capacity and offset.</summary>
        public Bag() : this(5, 5) {
        }
        /// <summary>Constructs a container with the specified capacity and the default offset.</summary>
        public Bag(int capacity) : this(capacity, 5) {
        }
        public Bag(T fill) : this(5, fill) {
        }
        /// <summary>Constructs a container with the specified capacity and offset.</summary>
        public Bag(int capacity, int offset) : this(capacity, offset, Environment.ProcessorCount) {
        }
        /// <summary>Constructs a container with the specified capacity, offset and concurrency.</summary>
        public Bag(int capacity, int offset, int concurrency) {
            this.Capacity = capacity;
            this.Offset = offset;
            this.Initialize(concurrency);
        }
        /// <summary>Constructs a container with the specified capacity and the default offset. Each element is a copy of the fill value.</summary>
        public Bag(int capacity, T fill) : this(capacity, 5, fill) {
        }
        /// <summary>Constructs a container with the specified capacity and offset. Each element is a copy of the fill value.</summary>
        public Bag(int capacity, int offset, T fill) : this(capacity, offset, Environment.ProcessorCount, fill) {
        }
        /// <summary>Constructs a container with the specified capacity, offset and concurrency. Each element is a copy of the fill value.</summary>
        public Bag(int capacity, int offset, int concurrency, T fill) {
            this.Capacity = capacity;
            this.Offset = offset;
            this.Initialize(concurrency, fill);
        }
        /// <summary>Constructs a container with a copy of each of the elements in array, in the same order.</summary>
        public Bag(IEnumerable<T> array) : this(5, array) {
        }
        /// <summary>Constructs a container with the specified offset and with a copy of each of the elements in array, in the same order.</summary>
        public Bag(int offset, IEnumerable<T> array) : this(offset, Environment.ProcessorCount, array) {
        }
        /// <summary>Constructs a container with the specified offset and concurrency and, with a copy of each of the elements in array, in the same order.</summary>
        public Bag(int offset, int concurrency, IEnumerable<T> array) {
            this.Capacity = 5;
            this.Offset = offset;
            this.Initialize(concurrency, array);
        }
        public int Add(T value) {
            int num = count;
            if (list.Enter(num, out object obj)) {
                Volatile.Write(ref list.Entries[num], new Entry<T>(value));
                count++;
                list.Exit(obj);
            }
            if (count == list.Entries.Length) {
                int num2 = list.Freeze();
                try {
                    this.Copy(list.Entries.Length + this.Offset, count);
                }
                finally {
                    list.Unfreeze(num2);
                }
            }
            return num;
        }
        /// <summary>Returns the specified element with bounds checking.</summary>
        public bool At(int index, out T value) {
            if (index < 0 || index > this.Count - 1) {
                value = default;
                return false;
            }
            value = this.Get(index);
            return true;
        }
        /// <summary>Returns iterator to beginning with the specified number of elements.</summary>
        public IEnumerable<T> Begin(int count) {
            Entry<T>[] array = list.Entries;
            for (int i = 0; i < this.GetBound(count); i++) {
                yield return Volatile.Read(ref array[i]).Value;
            }
        }
        public void Clear() {
            this.Clear(false);
        }
        public void Clear(bool trim) {
            int num = list.Freeze();
            try {
                if (count > 0) {
                    if (trim) {
                        list.Entries = new Entry<T>[this.Capacity];
                    }
                    else {
                        Array.Clear(list.Entries, 0, list.Entries.Length);
                    }
                    count = 0;
                }
            }
            finally {
                list.Unfreeze(num);
            }
        }
        public bool Contains(T value, out int index) {
            index = -1;
            for (int i = 0; i < list.Entries.Length; i++) {
                if (Volatile.Read(ref list.Entries[i]).Equals(value)) {
                    index = i;
                    return true;
                }
            }
            return false;
        }
        private void Copy(int size, int count) {
            Entry<T>[] array = new Entry<T>[size];
            this.CopyTo(array, 0, count);
            list.Entries = array;
        }
        private void CopyTo(Array array, int index, int count) {
            Array.Copy(list.Entries, 0, array, index, count);
        }
        /// <summary>Returns iterator to end.</summary>
        public IEnumerable<T> End() {
            return this.End(this.Count);
        }
        /// <summary>Returns iterator to end with the specified number of elements.</summary>
        public IEnumerable<T> End(int count) {
            Entry<T>[] array = list.Entries;
            for (int i = this.count - 1; i >= this.count - this.GetBound(count); i--) {
                yield return Volatile.Read(ref array[i]).Value;
            }
        }
        private T Get(int index) {
            Entry<T> e = Volatile.Read(ref list.Entries[index]);
            return e == null ? default : e.Value;
        }
        private int GetBound(int count) {
            return count > this.count ? this.count : count;
        }
        public IEnumerator<T> GetEnumerator() {
            return this.Begin(count).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        private void Initialize(int concurrency) {
            list = new Threads<Entry<T>>(concurrency, this.Capacity);
            count = 0;
        }
        private void Initialize(int concurrency, T fill) {
            list = new Threads<Entry<T>>(concurrency, this.Capacity);
            for (int i = 0; i < this.Capacity; i++) {
                list.Entries[i] = new Entry<T>(fill);
            }
            count = this.Capacity;
        }
        private void Initialize(int concurrency, IEnumerable<T> array) {
            if (array is Bag<T> bag) {
                int num = bag.list.Freeze();
                try {
                    list = new Threads<Entry<T>>(concurrency, bag.count);
                    bag.CopyTo(list.Entries, 0, bag.count);
                    count = bag.count;
                }
                finally {
                    bag.list.Unfreeze(num);
                }
            }
            else {
                list = new Threads<Entry<T>>(concurrency, this.Capacity);
                foreach (T e in array) {
                    this.Add(e);
                }
            }
        }
        public void Insert(int index, T value) {
            int num = list.Freeze();
            try {
                this.InsertCore(index, value);
            }
            finally {
                list.Unfreeze(num);
            }
        }
        /// <summary>Inserts copies of elements in array at the specified position.</summary>
        public void Insert(int index, IEnumerable<T> array) {
            int num = list.Freeze();
            try {
                this.InsertCore(index, array);
            }
            finally {
                list.Unfreeze(num);
            }
        }
        /// <summary>Inserts element at the specified position with bounds checking.</summary>
        public bool InsertAt(int index, T value) {
            if (index < 0) {
                return false;
            }
            int num = list.Freeze();
            try {
                if (index > count) {
                    return false;
                }
                this.InsertCore(index, value);
                return true;
            }
            finally {
                list.Unfreeze(num);
            }
        }
        /// <summary>Inserts copies of elements in array at the specified position with bounds checking.</summary>
        public bool InsertAt(int index, IEnumerable<T> array) {
            if (index < 0) {
                return false;
            }
            int num = list.Freeze();
            try {
                if (index > count) {
                    return false;
                }
                this.InsertCore(index, array);
                return true;
            }
            finally {
                list.Unfreeze(num);
            }
        }
        private void InsertCore(int index, T value) {
            this.ReserveCore(1);
            if (index < count) {
                Array.Copy(list.Entries, index, list.Entries, index + 1, count - index);
            }
            list.Entries[index] = new Entry<T>(value);
            count++;
        }
        private void InsertCore(int index, IEnumerable<T> array) {
            if (array is Bag<T> bag) {
                int num = bag.list.Freeze();
                try {
                    this.ReserveCore(bag.count);
                    if (index < count - 1) {
                        Array.Copy(list.Entries, index, list.Entries, index + bag.count, count - index);
                    }
                    bag.CopyTo(list.Entries, index, bag.count);
                    count = bag.count;
                }
                finally {
                    bag.list.Unfreeze(num);
                }
            }
            else {
                using (IEnumerator<T> e = array.GetEnumerator()) {
                    while (e.MoveNext()) {
                        this.InsertCore(index++, e.Current);
                    }
                }
            }
        }
        public void Remove(int index) {
            this.Remove(index, 1);
        }
        /// <summary>Removes the specified range of elements.</summary>
        public void Remove(int index, int count) {
            int num = list.Freeze();
            try {
                this.RemoveCore(index, count);
            }
            finally {
                list.Unfreeze(num);
            }
            
        }
        /// <summary>Removes the specified element with bounds checking. Returns true if removal succeeded.</summary>
        public bool RemoveAt(int index) {
            if (index < 0) {
                return false;
            }
            int num = list.Freeze();
            try {
                if (index > count - 1) {
                    return false;
                }
                this.RemoveCore(index, 1);
                return true;
            }
            finally {
                list.Unfreeze(num);
            }
        }
        /// <summary>Removes the specified range of elements with bounds checking. Returns true if removal succeeded.</summary>
        public bool RemoveAt(int index, int count) {
            if (index < 0) {
                return false;
            }
            int num = list.Freeze();
            try {
                if (index > count - 1) {
                    return false;
                }
                this.RemoveCore(index, count);
                return true;
            }
            finally {
                list.Unfreeze(num);
            }
        }
        private void RemoveCore(int index, int count) {
            this.count -= count;
            Array.Copy(list.Entries, index + count, list.Entries, index, this.count - index);
            if (count > 1) {
                Array.Clear(list.Entries, this.count, count);
            }
            else {
                list.Entries[count] = default;
            }
        }
        public void Reserve(int size) {
            int num = list.Freeze();
            try {
                this.ReserveCore(size);
            }
            finally {
                list.Unfreeze(num);
            }
        }
        private void ReserveCore(int size) {
            int num = list.Entries.Length - count;
            if (size > num) {
                this.ResizeCore(list.Entries.Length + (size - num));
            }
        }
        public void Resize(int size) {
            int num = list.Freeze();
            try {
                this.ResizeCore(size);
            }
            finally {
                list.Unfreeze(num);
            }
        }
        private void ResizeCore(int size) {
            int num = size;
            if (num > count) {
                num = count;
            }
            else {
                count = size;
            }
            this.Copy(size, num);
        }
        public void Reverse() {
            int num = list.Freeze();
            try {
                this.ReverseCore(0, count);
            }
            finally {
                list.Unfreeze(num);
            }
        }
        /// <summary>>Reverses the order of the elements in the specified range.</summary>
        public void Reverse(int index, int count) {
            int num = list.Freeze();
            try {
                this.ReverseCore(index, count);
            }
            finally {
                list.Unfreeze(num);
            }
        }
        /// <summary>>Reverses the order of the elements in the specified range with bounds checking.</summary>
        public bool ReverseAt(int index, int count) {
            if (index < 0) {
                return false;
            }
            int num = list.Freeze();
            try {
                if (index > this.count) {
                    return false;
                }
                this.ReverseCore(index, count);
                return true;
            }
            finally {
                list.Unfreeze(num);
            }
        }
        private void ReverseCore(int index, int count) {
            Array.Reverse(list.Entries, index, count);
        }
        /// <summary>Exchange contents of bags.</summary>
        public void Swap(Bag<T> bag) {
            int num = list.Freeze();
            int num2 = bag.list.Freeze();
            try {
                int num3 = count;
                Entry<T>[] array = this.Swap(list.Entries, list.Entries.Length);
                list.Entries = this.Swap(bag.list.Entries, bag.list.Entries.Length);
                count = bag.count;
                bag.list.Entries = array;
                bag.count = num3;
            }
            finally {
                list.Unfreeze(num);
                bag.list.Unfreeze(num2);
            }
        }
        private Entry<T>[] Swap(Array source, int size) {
            Entry<T>[] array = new Entry<T>[size];
            Array.Copy(source, 0, array, 0, size);
            return array;
        }
        public void Trim() {
            int num = list.Freeze();
            try {
                if (list.Entries.Length > count) {
                    this.ResizeCore(count);
                }
            }
            finally {
                list.Unfreeze(num);
            }
        }
        /// <summary>Copies the elements of the container to an array.</summary>
        public T[] ToArray() {
            int num = this.Count;
            T[] array = new T[num];
            if (num > 0) {
                for (int i = 0; i < num; i++) {
                    array[i] = Volatile.Read(ref list.Entries[i]).Value;
                }
            }
            return array;
        }
        private void Update(int index, T value) {
            if (list.Enter(index, out object obj)) {
                Volatile.Write(ref list.Entries[index], new Entry<T>(value));
                list.Exit(obj);
            }
        }
    }
}
