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
// A last-in-first-out (LIFO) contiguous container of T elements.
//
// Similar implementation as Vector<T>.
// 
// The container is shrinked to its initial capacity when all elements are 
// popped out. 


using System;
using System.Collections;
using System.Collections.Generic;
namespace Circus.Collections {
    /// <summary>Provides a last-in-first-out (LIFO) contiguous container of T elements.</summary>
    public class Stack<T> : IStack<T>, IEnumerable<T>, ICollection {
        private T[] array;
        private IEqualityComparer<T> comparer;
        public int Capacity { get; private set; }
        public int Count { get; private set; }
        public bool Empty => this.Count == 0;
        public bool IsSynchronized => false;
        public int Offset { get; private set; }
        public int Size => this.array.Length;
        public object SyncRoot => throw new NotImplementedException();
        /// <summary>Constructs a container with the default capacity and offset.</summary>
        public Stack() : this(5, 5) {
        }
        /// <summary>Constructs a container with the specified capacity and the default offset.</summary>
        public Stack(int capacity) : this(capacity, 5) {
        }
        /// <summary>Constructs a container with a copy of each of the elements in array.</summary>
        public Stack(IEnumerable<T> array) : this(5, array) {
        }
        /// <summary>Constructs a container with the specified equality comparer.</summary>
        public Stack(IEqualityComparer<T> comparer) : this(5, 5, comparer) {
        }
        /// <summary>Constructs a container with the specifed capacity and offset.</summary>
        public Stack(int capacity, int offset) : this(capacity, offset, EqualityComparer<T>.Default) {
        }
        /// <summary>Constructs a container with the specified offset and a copy of each of the elements in array.</summary>
        public Stack(int offset, IEnumerable<T> array) : this(offset, array, EqualityComparer<T>.Default) {
        }
        /// <summary>Constructs a container with the specified capacity and equality comparer.</summary>
        public Stack(int capacity, IEqualityComparer<T> comparer) : this(capacity, 5, comparer) {
        }
        /// <summary>Constructs a container with a copy of each of the elements in array and the specified equality comparer.</summary>
        public Stack(IEnumerable<T> array, IEqualityComparer<T> comparer) : this(5, array, comparer) {
        }
        /// <summary>Constructs a container with the specified capacity, offset and equality comparer.</summary>
        public Stack(int capacity, int offset, IEqualityComparer<T> comparer) {
            this.Capacity = capacity;
            this.comparer = comparer;
            this.Offset = offset;
            this.Initialize(true);
        }
        /// <summary>Constructs a container with the specified offset, a copy of each of the elements in array and the specified equality comparer.</summary>
        public Stack(int offset, IEnumerable<T> array, IEqualityComparer<T> comparer) {
            this.comparer = comparer;
            this.Offset = offset;
            this.Initialize(array);
        }
        /// <summary>Returns iterator to beginning with the specified number of elements.</summary>
        public IEnumerable<T> Begin(int count) {
            for (int i = 0; i < this.GetBound(count); i++) {
                yield return this.array[i];
            }
        }
        public void Clear() {
            this.Clear(false);
        }
        public void Clear(bool trim) {
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
        public void CopyTo(Array array, int index) {
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
                    this.Push(e);
                }
            }
        }
        private void InitializeCore() {
            this.comparer = typeof(string).IsAssignableFrom(typeof(T)) ? (IEqualityComparer<T>)(IEqualityComparer<string>)new Text.StringComparer() : EqualityComparer<T>.Default;
        }
        public T Peek() {
            return this.Get(this.Count - 1);
        }
        public bool Pop(out T value) {
            if (this.Count > 0) {
                int num = this.Count - 1;
                value = this.Get(num);
                return !(this.Reclaim(num) && this.Reset());
            }
            value = default;
            return false;
        }
        public bool Push(T value) {
            int num = this.Count;
            if (num == this.Size) {
                this.Copy(this.Size + this.Offset, num);
            }
            this.array[num] = value;
            this.Count++;
            return true;
        }
        private bool Reclaim(int index) {
            this.array[index] = default;
            return this.Count-- == 0;
        }
        public void Remove(int index) {
            this.Count--;
            Array.Copy(this.array, index + 1, this.array, index, this.Count - index);
            this.array[this.Count] = default;
        }
        /// <summary>Removes the specified element with bounds checking. Returns true if removal succeeded.</summary>
        public bool RemoveAt(int index) {
            if (index < 0 || index > this.Count - 1) {
                return false;
            }
            this.Remove(index);
            return true;
        }
        public void Reserve(int size) {
            int num = this.Size - this.Count;
            if (size > num) {
                this.Resize(this.Size + (size - num));
            }
        }
        private bool Reset() {
            if (this.Size > this.Capacity) {
                this.array = new T[this.Capacity];
            }
            return true;
        }
        public void Resize(int size) {
            int num = size;
            if (num > this.Count) {
                num = this.Count;
            }
            else {
                this.Count = size;
            }
            this.Copy(size, num);
        }
        public void Reverse() {
            Array.Reverse(this.array, 0, this.Count);
        }
        /// <summary>Exchange contents of stacks.</summary>
        public void Swap(Stack<T> stack) {
            int num = this.Count;
            T[] array = this.Swap(this.array, this.Size);
            this.array = this.Swap(stack.array, stack.Size);
            this.Count = stack.Count;
            stack.array = array;
            stack.Count = num;
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
