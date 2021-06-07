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
// An unordered associative container of T keys and U values.
//
// Same implementation as Set<T>.


using System;
using System.Collections;
using System.Collections.Generic;
using Circus.Text;
namespace Circus.Collections {
    /// <summary>Provides an unordered associative container of T keys and U values. Default capacity and offset is 5.</summary>
    [Serializable]
    public class Map<T, U> : IMap<T, U>, IEnumerable<KeyValuePair<T, U>> {
        private class Node : Entry<U> {
            internal ulong Hash;
            internal T Key;
            internal Node Next;
            internal Node(ulong hash, T key, Node next, U value) : base(value) {
                this.Hash = hash;
                this.Key = key;
                this.Next = next;
            }
        }
        private Node[] array;
        private IEqualityComparer<T> comparer;
        private bool flag;
        public U this[T key] { get => this.Get(key, out U value) ? value : default; set => this.Add(key, value, false); }
        public int Capacity { get; private set; }
        public int Count { get; private set; }
        public bool Empty => this.Count == 0;
        public int Offset { get; private set; }
        public int Size => this.array.Length;
        /// <summary>Constructs a container with the default capacity and offset.</summary>
        public Map() : this(5, 5) {
        }
        /// <summary>Constructs a container with the specified capacity and the default offset.</summary>
        public Map(int capacity) : this(capacity, 5) { 
        }
        /// <summary>Constructs a container with a copy of each of the elements in array.</summary>
        public Map(IEnumerable<KeyValuePair<T, U>> array) : this(5, array) {
        }
        /// <summary>Constructs a container with the specified equality comparer.</summary>
        public Map(IEqualityComparer<T> comparer) : this(5, 5, comparer) {
        }
        /// <summary>Constructs a container with a copy of the elements in map.</summary>
        public Map(Map<T, U> map) : this(map.Offset, map) {
        }
        /// <summary>Constructs a container with the specifed capacity and offset.</summary>
        public Map(int capacity, int offset) : this(capacity, offset, EqualityComparer<T>.Default) {
        }
        /// <summary>Constructs a container with the specified offset and a copy of each of the elements in array.</summary>
        public Map(int offset, IEnumerable<KeyValuePair<T, U>> array) : this(offset, array, EqualityComparer<T>.Default) {
        }
        /// <summary>Constructs a container with the specified capacity and equality comparer.</summary>
        public Map(int capacity, IEqualityComparer<T> comparer) : this(capacity, 5, comparer) {
        }
        /// <summary>Constructs a container with the specified offset and a copy of the elements in map.</summary>
        public Map(int offset, Map<T, U> map) {
            this.Offset = offset;
            this.InitializeCore(map.Capacity);
            this.Initialize(map);
        }
        /// <summary>Constructs a container with a copy of each of the elements in array and the specified equality comparer.</summary>
        public Map(IEnumerable<KeyValuePair<T, U>> array, IEqualityComparer<T> comparer) : this(5, array, comparer) {
        }
        /// <summary>Constructs a container with the specified capacity, offset and equality comparer.</summary>
        public Map(int capacity, int offset, IEqualityComparer<T> comparer) {
            this.comparer = comparer;
            this.Offset = offset;
            this.InitializeCore(capacity);
            this.Initialize();
        }
        /// <summary>Constructs a container with the specified offset, a copy of each of the elements in array and the specified equality comparer.</summary>
        public Map(int offset, IEnumerable<KeyValuePair<T, U>> array, IEqualityComparer<T> comparer) {
            this.comparer = comparer;
            this.Offset = offset;
            this.InitializeCore(5);
            this.Initialize(array);
        }
        public bool Add(T key, U value) {
            return this.Add(key, value, false);
        }
        private bool Add(T key, U value, bool update) {
            bool f = this.Find(key, out Node node, out Bucket<ulong> bucket);
            if (f) {
                if (update) {
                    node.Value = value;
                    return true;
                }
                return false;
            }
            return this.Add(key, value, bucket);
        }
        private bool Add(T key, U value, Bucket<ulong> bucket) {
            this.array[bucket.Index] = new Node(bucket.Hash, key, this.array[bucket.Index], value);
            this.Count++;
            if (this.Count == this.Size) {
                this.ResizeCore(this.Size + this.Offset);
            }
            return true;
        }
        /// <summary>Inserts the specified element to the container. If the key already exists, updates its value.</summary>
        public void AddOrUpdate(T key, U value) {
            this.Add(key, value, true);
        }
        public void Clear() {
            this.Clear(false);
        }
        public void Clear(bool trim) {
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
        public bool Contains(T key) {
            return this.Find(key, out _, out _);
        }
        private bool Find(T key, out Node node, out Bucket<ulong> bucket) {
            bucket = this.GetBucketInfo(key);
            for (Node n = this.array[bucket.Index]; n != null; n = n.Next) {
                if (comparer.Equals(key, n.Key)) {
                    node = n;
                    return true;
                }
            }
            node = null;
            return false;
        }
        public bool Get(T key, out U value) {
            bool f = this.Find(key, out Node node, out _);
            value = f ? node.Value : default;
            return f;
        }
        private Bucket<ulong> GetBucketInfo(T key) {
            ulong num = this.flag ? StringInfo.GetHash(key.ToString()) : (ulong)(key.GetHashCode() & int.MaxValue);
            return new Bucket<ulong>(num, (int)(num % (ulong)this.array.Length));
        }
        public IEnumerator<KeyValuePair<T, U>> GetEnumerator() {
            for (int i = 0; i < this.array.Length; i++) {
                for (Node node = this.array[i]; node != null; node = node.Next) {
                    yield return new KeyValuePair<T, U>(node.Key, node.Value);
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        /// <summary>Accesses the element with the specified key. If the key doesn't exists, the element is added to the container.</summary>
        public U GetOrAdd(T key, U value) {
            bool flag = this.Find(key, out Node node, out Bucket<ulong> bucket);
            if (!flag) {
                this.Add(key, value, bucket);
            }
            return flag ? node.Value : value;
        }
        private void Initialize() {
            this.array = new Node[this.Capacity];
            this.Count = 0;
        }
        private void Initialize(Map<T, U> map) {
            this.array = new Node[map.Size];
            Array.Copy(map.array, 0, this.array, 0, map.Size);
            this.Count = map.Count;
        }
        private void Initialize(IEnumerable<KeyValuePair<T, U>> array) {
            this.array = new Node[this.Capacity];
            foreach (KeyValuePair<T, U> pair in array) {
                this.Add(pair.Key, pair.Value);
            }
        }
        private void InitializeCore(int capacity) {
            if (typeof(string).IsAssignableFrom(typeof(T)) && comparer == EqualityComparer<T>.Default) {
                this.comparer = (IEqualityComparer<T>)(IEqualityComparer<string>)new Text.StringComparer();
                flag = true;
            }
            this.Capacity = capacity != 5 ? Numeric.NextPrime(capacity) : capacity;
        }
        public IEnumerable<T> Keys() {
            for (int i = 0; i < this.array.Length; i++) {
                for (Node node = this.array[i]; node != null; node = node.Next) {
                    yield return node.Key;
                }
            }
        }
        public bool Remove(T key) {
            if (!this.Find(key, out Node node, out Bucket<ulong> bucket)) {
                return false;
            }
            this.array[bucket.Index] = node.Next ?? null;
            this.Count--;
            return true;
        }
        public void Reserve(int size) {
            this.ResizeCore(this.Size + (size - this.Size - this.Count));
        }
        public void Resize(int size) {
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
                        array[num] = new Node(current.Hash, current.Key, array[num], current.Value);
                        current = next;
                    }
                }
            }
            this.array = array;
        }
        /// <summary>Exchange contents of maps.</summary>
        public void Swap(Map<T, U> map) {
            int num = this.Count;
            Node[] array = this.Swap(this.array, this.Size);
            this.array = this.Swap(map.array, map.Size);
            this.Count = map.Count;
            map.array = array;
            map.Count = num;
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
        /// <summary>Copies the elements of the container to an array of KeyValuePairs.</summary>
        public KeyValuePair<T, U>[] ToArray() {
            KeyValuePair<T, U>[] array = new KeyValuePair<T, U>[this.Count];
            int num = 0;
            for (int i = 0; i < this.array.Length; i++) {
                for (Node node = this.array[i]; node != null; node = node.Next) {
                    array[num] = new KeyValuePair<T, U>(node.Key, node.Value);
                    num++;
                }
            }
            return array;
        }
        public IEnumerable<U> Values() {
            for (int i = 0; i < this.array.Length; i++) {
                for (Node node = this.array[i]; node != null; node = node.Next) {
                    yield return node.Value;
                }
            }
        }
    }
}
