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
// A dynamic contiguous container of T elements. 
//
// The purpose was to have an improved version of List<T> that implements
// fill constructors, back and forth iterators, unchecked accessors, content 
// swapping and a better control on memory allocation.
//
// Constructors provide an initial capacity and the additional amount of 
// memory that will be allocated to handle future growth. These values are 
// read-only and therefore cannot be modified after the instance is created. 
// If a larger amount of memory is required during execution, use Reserve() 
// or Resize() methods to allocate the desired space.
//
// Fill constructors initialize the content with a copy of the provided value.
// Using fill constructors when T is of type Int32 can lead to some confusion
// picking up the correct overloaded ctor. If so, specify the parameter name
// to avoid undesired behavior like this:
//
// Vector<int> vector = new Vector<int>(fill: 1);
//
// Range constructors initialize the content with a copy of the specified
// IEnumerable.
//
// Since Vector implements IEnumerable, it is possible to initialize from an 
// instance of Vector. ICollection is implemented as it provides the CopyTo
// method to perform Array.Copy() (which is in fact an internal framework call
// to an unmanaged memmove) instead of loops. IsSynchronized and SyncRoot are
// part of ICollection but are not implemented. Use Bag instead if thread
// safety is needed.
//
// Reserve() will increase by the specified number of items, whereas Resize() 
// can expand or contract to the specified value. In case of contraction, 
// existing elements outside the new upper bound are removed from the container.
//
// Index accessor, Insert(), Remove() and Reverse() do not perform any bounds 
// checking. Use At(), InsertAt(), RemoveAt() and ReverseAt() instead if bounds 
// safety is required.
//
// The default value for capacity and offset is 5.


using System;
using System.Collections;
using System.Collections.Generic;
namespace Circus.Collections {
    /// <summary>Provides a contiguous container of T elements. Default capacity and offset is 5.</summary>
    [Serializable]
    public class Vector<T> : IVector<T>, IEnumerable<T>, ICollection {
        private T[] array;
        private IEqualityComparer<T> comparer;
        public virtual T this[int index] { get => this.Get(index); set => this.array[index] = value; }
        public T Back { get => this.Count > 0 ? this.Get(this.Count - 1) : this.Front; set => this.array[this.Count > 0 ? this.Count - 1 : 0] = value; }
        public int Capacity { get; private set; }
        public int Count { get; private set; }
        public bool Empty => this.Count == 0;
        public T Front { get => this.Get(0); set => this.array[0] = value; }
        public bool IsSynchronized => false;
        public int Offset { get; private set; }
        public int Size => this.array.Length;
        public object SyncRoot => throw new NotImplementedException();
        /// <summary>Constructs a container with the default capacity and offset.</summary>
        public Vector() : this(5, 5) {
        }
        /// <summary>Constructs a container with the specified capacity and the default offset.</summary>
        public Vector(int capacity) : this(capacity, 5) {
        }
        public Vector(T fill) : this(5, fill) {
        }
        /// <summary>Constructs a container with the specified capacity and offset.</summary>
        public Vector(int capacity, int offset) {
            this.Capacity = capacity;
            this.Offset = offset;
            this.Initialize(true);
        }
        /// <summary>Constructs a container with the specified capacity and the default offset. Each element is a copy of the fill value.</summary>
        public Vector(int capacity, T fill) : this(capacity, 5, fill) {
        }
        /// <summary>Constructs a container with the specified capacity and offset. Each element is a copy of the fill value.</summary>
        public Vector(int capacity, int offset, T fill) {
            this.Capacity = capacity;
            this.Offset = offset;
            this.Initialize(fill);
        }
        /// <summary>Constructs a container with a copy of each of the elements in array, in the same order.</summary>
        public Vector(IEnumerable<T> array) : this(5, array) { 
        }
        /// <summary>Constructs a container with the specified offset and with a copy of each of the elements in array, in the same order.</summary>
        public Vector(int offset, IEnumerable<T> array) {
            this.Capacity = 5;
            this.Offset = offset;
            this.Initialize(array);
        }
        public virtual int Add(T value) {
            int num = this.Count;
            if (num == this.Size) {
                this.Copy(this.Size + this.Offset, num);
            }
            this.array[num] = value;
            this.Count++;
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
            for (int i = 0; i < this.GetBound(count); i++) {
                yield return this.array[i];
            }
        }
        public virtual void Clear() {
            this.Clear(false);
        }
        public virtual void Clear(bool trim) {
            if (this.Count > 0) {
                if (trim) { 
                    this.Initialize(false); 
                } 
                else { 
                    Array.Clear(this.array, 0, this.Size);
                    this.Count = 0;
                }
            }
        }
        public bool Contains(T value, out int index) {
            index = -1;
            for (int i = 0; i < this.array.Length; i++) {
                if (this.comparer.Equals(this.array[i], value)) {
                    index = i;
                    return true;
                }
            }
            return false;
        }
        private void Copy(int size, int count) {
            T[] array = new T[size];
            this.CopyTo(array, 0, count);
            this.array = array;
        }
        public virtual void CopyTo(Array array, int index) {
            this.CopyTo(array, index, this.Count);
        }
        private void CopyTo(Array array, int index, int count) {
            Array.Copy(this.array, 0, array, index, count);
        }
        /// <summary>Returns iterator to end.</summary>
        public IEnumerable<T> End() {
            return this.End(this.Count);
        }
        /// <summary>Returns iterator to end with the specified number of elements.</summary>
        public IEnumerable<T> End(int count) {
            for (int i = this.Count - 1; i >= this.Count - this.GetBound(count); i--) {
                yield return this.array[i];
            }
        }
        private T Get(int index) {
            T e = this.array[index];
            return e == null ? default : e;
        }
        private int GetBound(int count) {
            return count > this.Count ? this.Count : count;
        }
        public IEnumerator<T> GetEnumerator() {
            return this.Begin(this.Count).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        private void Initialize(bool flag) {
            if (flag) {
                this.InitializeCore();
            }
            this.array = new T[this.Capacity];
            this.Count = 0;
        }
        private void Initialize(T fill) {
            this.InitializeCore();
            this.array = new T[this.Capacity];
            for (int i = 0; i < this.Capacity; i++) {
                this.array[i] = fill;
            }
            this.Count = this.Capacity;
        }
        private void Initialize(IEnumerable<T> array) {
            this.InitializeCore();
            if (array is ICollection collection) {
                this.array = new T[collection.Count];
                collection.CopyTo(this.array, 0);
                this.Count = collection.Count;
            }
            else {
                this.array = new T[this.Capacity];
                foreach (T e in array) {
                    this.Add(e);
                }
            }
        }
        private void InitializeCore() {
            this.comparer = typeof(string).IsAssignableFrom(typeof(T)) ? (IEqualityComparer<T>)(IEqualityComparer<string>)new Text.StringComparer() : EqualityComparer<T>.Default;
        }
        public virtual void Insert(int index, T value) {
            this.Reserve(1);
            if (index < this.Count) {
                Array.Copy(this.array, index, this.array, index + 1, this.Count - index);
            }
            this.array[index] = value;
            this.Count++;
        }
        /// <summary>Inserts copies of elements in array at the specified position.</summary>
        public virtual void Insert(int index, IEnumerable<T> array) {
            if (array is ICollection collection) {
                this.Reserve(collection.Count);
                if (index < this.Count - 1) {
                    Array.Copy(this.array, index, this.array, index + collection.Count, this.Count - index);
                }
                collection.CopyTo(this.array, index);
                this.Count += collection.Count;
            }
            else {
                using (IEnumerator<T> e = array.GetEnumerator()) {
                    while (e.MoveNext()) {
                        this.Insert(index++, e.Current);
                    }
                }
            }
        }
        /// <summary>Inserts element at the specified position with bounds checking.</summary>
        public virtual bool InsertAt(int index, T value) {
            if (index < 0 || index > this.Count) {
                return false;
            }
            this.Insert(index, value);
            return true;
        }
        /// <summary>Inserts copies of elements in array at the specified position with bounds checking.</summary>
        public virtual bool InsertAt(int index, IEnumerable<T> array) {
            if (index < 0 || index > this.Count) {
                return false;
            }
            this.Insert(index, array);
            return true;
        }
        public virtual void Remove(int index) {
            this.Count--;
            Array.Copy(this.array, index + 1, this.array, index, this.Count - index);
            this.array[this.Count] = default;
        }
        /// <summary>Removes the specified range of elements.</summary>
        public virtual void Remove(int index, int count) {
            this.Count -= count;
            Array.Copy(this.array, index + count, this.array, index, this.Count - index);
            Array.Clear(this.array, this.Count, count);
        }
        /// <summary>Removes the specified element with bounds checking. Returns true if removal succeeded.</summary>
        public virtual bool RemoveAt(int index) {
            if (index < 0 || index > this.Count - 1) {
                return false;
            }
            this.Remove(index);
            return true;
        }
        /// <summary>Removes the specified range of elements with bounds checking. Returns true if removal succeeded.</summary>
        public virtual bool RemoveAt(int index, int count) {
            if (index < 0 || index + count > this.Count - 1) {
                return false;
            }
            this.Remove(index, count);
            return true;
        }
        public void Reserve(int size) {
            int num = this.Size - this.Count;
            if (size > num) {
                this.Resize(this.Size + (size - num));
            }
        }
        public virtual void Resize(int size) {
            int num = size;
            if (num > this.Count) {
                num = this.Count;
            }
            else {
                this.Count = size;
            }
            this.Copy(size, num);
        }
        public virtual void Reverse() {
            this.Reverse(0, this.Count);
        }
        /// <summary>Reverses the order of the elements in the specified range.</summary>
        public virtual void Reverse(int index, int count) {
            Array.Reverse(this.array, index, count);
        }
        /// <summary>Reverses the order of the elements in the specified range with bounds checking.</summary>
        public virtual bool ReverseAt(int index, int count) {
            if (index < 0 || index > this.Count) {
                return false;
            }
            this.Reverse(index, count);
            return true;
        }
        /// <summary>Exchange contents of vectors.</summary>
        public virtual void Swap(Vector<T> vector) {
            int num = this.Count;
            T[] array = this.Swap(this.array, this.Size);
            this.array = this.Swap(vector.array, vector.Size);
            this.Count = vector.Count;
            vector.array = array;
            vector.Count = num;
        }
        private T[] Swap(Array source, int size) {
            T[] array = new T[size];
            Array.Copy(source, 0, array, 0, size);
            return array;
        }
        public void Trim() {
            if (this.Size > this.Count) {
                this.Resize(this.Count);
            }
        }
        /// <summary>Copies the elements of the container to an array.</summary>
        public T[] ToArray() {
            T[] array = new T[this.Count];
            if (this.Count > 0) {
                this.CopyTo(array, 0, this.Count);
            }
            return array;
        }
    }
}