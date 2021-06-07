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
// An unordered container of unique T elements.
//
// The purpose was to have an improved version of HashSet<T> and especially
// when dealing with strings.
//
// It uses Google's Farmhash for string hashing which provides a better 
// distribution and therefore avoids a cost effective rehashing since the number 
// of collisions is kept low.
//
// Collisions are handled with a linked list instead of a bucket list. This 
// avoids filling the bucket list with a loop on each resize.
//
// String comparison is provided by an internal StringComparer that uses a 
// Boyer-Moore like method instead of the standard .net naive search. Performance
// is comparable on small strings (up to 20 chars) but improved by 20-30% on 
// medium to large strings.
//
// Like other containers, Set allows to define an offset for a better control on 
// memory allocation, but the number of reserved buckets is increased to the next 
// upper prime to maintain collisions frequency at lowest. This is why the actual 
// size may be greater than the previous size + offset.


using System;
using System.Collections;
using System.Collections.Generic;
using Circus.Text;
namespace Circus.Collections {
    /// <summary>Provides an unordered container of unique T elements. Default capacity and offset is 5.</summary>
    [Serializable]
    public class Set<T> : ISet<T>, IEnumerable<T> {
        private class Node : Entry<T> {
            internal ulong Hash;
            internal Node Next;
            internal Node(ulong hash, Node next, T value) : base(value) {
                this.Hash = hash;
                this.Next = next;
            }
        }
        private Node[] array;
        private IEqualityComparer<T> comparer;
        private bool flag;
        public T this[T value] {
            get {
                return this.Contains(value) ? value : default;
            }
        }
        public int Capacity { get; private set; }
        public int Count { get; private set; }
        public bool Empty => this.Count == 0;
        public int Offset { get; private set; }
        public int Size => this.array.Length;
        /// <summary>Constructs a container with the default capacity and offset.</summary>
        public Set() : this(5, 5) {
        }
        /// <summary>Constructs a container with the specified capacity and the default offset.</summary>
        public Set(int capacity) : this(capacity, 5) {
        }
        /// <summary>Constructs a container with a copy of each of the elements in array.</summary>
        public Set(IEnumerable<T> array) : this(5, array) {
        }
        /// <summary>Constructs a container with the specified equality comparer.</summary>
        public Set(IEqualityComparer<T> comparer) : this(5, 5, comparer) {
        }
        /// <summary>Constructs a container with the specifed capacity and offset.</summary>
        public Set(int capacity, int offset) : this(capacity, offset, EqualityComparer<T>.Default) {
        }
        /// <summary>Constructs a container with the specified offset and a copy of each of the elements in array.</summary>
        public Set(int offset, IEnumerable<T> array) : this(offset, array, EqualityComparer<T>.Default) {
        }
        /// <summary>Constructs a container with the specified capacity and equality comparer.</summary>
        public Set(int capacity, IEqualityComparer<T> comparer) : this(capacity, 5, comparer) {
        }
        /// <summary>Constructs a container with a copy of each of the elements in array and the specified equality comparer.</summary>
        public Set(IEnumerable<T> array, IEqualityComparer<T> comparer) : this(5, array, comparer) {
        }
        /// <summary>Constructs a container with the specified capacity, offset and equality comparer.</summary>
        public Set(int capacity, int offset, IEqualityComparer<T> comparer) {
            this.comparer = comparer;
            this.Offset = offset;
            this.InitializeCore(capacity);
            this.Initialize();
        }
        /// <summary>Constructs a container with the specified offset, a copy of each of the elements in array and the specified equality comparer.</summary>
        public Set(int offset, IEnumerable<T> array, IEqualityComparer<T> comparer) {
            this.comparer = comparer;
            this.Offset = offset;
            this.InitializeCore(5);
            this.Initialize(array);
        }
        public virtual bool Add(T value) {
            if (this.Find(value, out _, out Bucket<ulong> bucket)) {
                return false;
            }
            this.array[bucket.Index] = new Node(bucket.Hash, this.array[bucket.Index], value);
            this.Count++;
            if (this.Count == this.Size) {
                this.ResizeCore(this.Size + this.Offset);
            }
            return true;
        }
        public virtual void Clear() {
            this.Clear(false);
        }
        public virtual void Clear(bool trim) {
            if (this.Count > 0) {
                if (trim) {
                    this.Initialize();
                }
                else {
                    Array.Clear(this.array, 0, this.Size);
                    this.Count = 0;
                }
            }
        }
        public bool Contains(T value) {
            return this.Find(value, out _, out _);
        }
        private bool Find(T value, out Node node, out Bucket<ulong> bucket) {
            bucket = this.GetBucketInfo(value);
            for (Node n = this.array[bucket.Index]; n != null; n = n.Next) {
                if (comparer.Equals(value, n.Value)) {
                    node = n;
                    return true;
                }
            }
            node = null;
            return false;
        }
        private Bucket<ulong> GetBucketInfo(T key) {
            ulong num = this.flag ? StringInfo.GetHash(key.ToString()) : (ulong)(key.GetHashCode() & int.MaxValue);
            return new Bucket<ulong>(num, (int)(num % (ulong)this.array.Length));
        }
        public IEnumerator<T> GetEnumerator() {
            for (int i = 0; i < this.array.Length; i++) {
                for (Node node = this.array[i]; node != null; node = node.Next) {
                    yield return node.Value;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        private void Initialize() {
            this.array = new Node[this.Capacity];
            this.Count = 0;
        }
        private void Initialize(IEnumerable<T> array) {
            if (array is Set<T> set) {
                this.flag = set.flag;
                this.Capacity = set.Capacity;
                this.array = new Node[set.Size];
                Array.Copy(set.array, 0, this.array, 0, set.array.Length);
                this.Count = set.Count;
            }
            else {
                this.array = new Node[this.Capacity];
                foreach (T value in array) {
                    this.Add(value);
                }
            }
        }
        private void InitializeCore(int capacity) {
            if (typeof(string).IsAssignableFrom(typeof(T)) && comparer == EqualityComparer<T>.Default) {
                this.comparer = (IEqualityComparer<T>)(IEqualityComparer<string>)new Text.StringComparer();
                flag = true;
            }
            if (capacity != 5) {
                capacity = Numeric.NextPrime(capacity);
            }
            this.Capacity = capacity;
        }
        public virtual bool Remove(T value) {
            if (!this.Find(value, out Node node, out Bucket<ulong> bucket)) {
                return false;
            }
            this.array[bucket.Index] = node.Next ?? null;
            this.Count--;
            return true;
        }
        public void Reserve(int size) {
            this.ResizeCore(this.Size + (size - this.Size - this.Count));
        }
        public virtual void Resize(int size) {
            if (size < this.Count) {
                return;
            }
            this.ResizeCore(size);
        }
        private void ResizeCore(int size) {
            size = Numeric.NextPrime(size);
            Node[] array = new Node[size];
            for (int i = 0; i < this.array.Length; i++) {
                Node current = this.array[i];
                checked {
                    while (current != null) {
                        Node next = current.Next;
                        int num = (int)(current.Hash % (ulong)array.Length);
                        array[num] = new Node(current.Hash, array[num], current.Value);
                        current = next;
                    }
                }
            }
            this.array = array;
        }
        /// <summary>Exchange contents of sets.</summary>
        public virtual void Swap(Set<T> set) {
            int num = this.Count;
            Node[] array = this.Swap(this.array, this.Size);
            this.array = this.Swap(set.array, set.Size);
            this.Count = set.Count;
            set.array = array;
            set.Count = num;
        }
        private Node[] Swap(Array source, int size) {
            Node[] array = new Node[size];
            Array.Copy(source, 0, array, 0, size);
            return array;
        }
        public void Trim() {
            if (this.Count > 0) {
                this.ResizeCore(this.Count);
            }
        }
        /// <summary>Copies the elements of the container to an array.</summary>
        public T[] ToArray() {
            T[] array = new T[this.Count];
            int num = 0;
            for (int i = 0; i < this.array.Length; i++) {
                for (Node node = this.array[i]; node != null; node = node.Next) {
                    array[num] = node.Value;
                    num++;
                }
            }
            return array;
        }
    }
}
