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
// A map of weak references T keys and U values.
//
// Keys are encapsulated in an Entry class that provides a weak reference
// to the source object and are compared by their source hash code and 
// reference state (IsAlive).
//
// A dead reference is removed from the container when accessed, which 
// includes: accessor, Add(), AddOrUpdate(), Contains(), Get() and 
// GetOrAdd().
//
// Enumerations like GetEnumerator(), Keys(), ToArray() and Values() do not 
// return dead references but to preserve performance, they are kept in the
// container until they are accessed using the above-mentioned accessors.
//
// Reserve(), Resize(), Swap(), Trim() and initialization from an
// IEnumerable or a WeakMap do not perform dead references checks.
//
// Since keys are weak references, T must be a reference type.


using System;
using System.Collections;
using System.Collections.Generic;
namespace Circus.Collections.Conditional {
    /// <summary>Provides a map of weak references T keys and U values.</summary>
    public class WeakMap<T, U> : IMap<T, U>, IEnumerable<KeyValuePair<T, U>> where T : class {
        private class Node : Collections.Entry<U> {
            internal Entry<T> Key;
            internal Node Next;
            internal Node(Entry<T> key, Node next, U value) : base(value) {
                this.Key = key;
                this.Next = next;
            }
        }
        private Node[] array;
        public U this[T key] { get => this.Get(key, out U value) ? value : default; set => this.Add(key, value, false); }
        public int Capacity { get; private set; }
        public int Count { get; private set; }
        public bool Empty => this.Count == 0;
        public int Offset { get; private set; }
        public int Size => this.array.Length;
        /// <summary>Constructs a container with the default capacity and offset.</summary>
        public WeakMap() : this(5, 5) {
        }
        /// <summary>Constructs a container with the specified capacity and the default offset.</summary>
        public WeakMap(int capacity) : this(capacity, 5) {
        }
        /// <summary>Constructs a container with a copy of each of the elements in array.</summary>
        public WeakMap(IEnumerable<KeyValuePair<T, U>> array) : this(5, array) {
        }
        /// <summary>Constructs a container with a copy of the elements in weak map.</summary>
        public WeakMap(WeakMap<T, U> map) : this(map.Offset, map) {
        }
        /// <summary>Constructs a container with the specifed capacity and offset.</summary>
        public WeakMap(int capacity, int offset) {
            this.Offset = offset;
            this.InitializeCore(capacity);
            this.Initialize();
        }
        /// <summary>Constructs a container with the specified offset and a copy of each of the elements in array.</summary>
        public WeakMap(int offset, IEnumerable<KeyValuePair<T, U>> array) {
            this.Offset = offset;
            this.InitializeCore(5);
            this.Initialize(array);
        }
        /// <summary>Constructs a container with the specified offset and a copy of the elements in weak map.</summary>
        public WeakMap(int offset, WeakMap<T, U> map) {
            this.Offset = offset;
            this.InitializeCore(map.Capacity);
            this.Initialize(map);
        }
        public bool Add(T key, U value) {
            return this.Add(key, value, false);
        }
        private bool Add(T key, U value, bool update) {
            bool num = this.Find(key, out Node node, out Bucket<int> bucket);
            if (num) {
                if (update) {
                    node.Value = value;
                    return true;
                }
                return false;
            }
            return this.Add(key, value, bucket);
        }
        private bool Add(T key, U value, Bucket<int> bucket) {
            this.array[bucket.Index] = new Node(new Entry<T>(key), this.array[bucket.Index], value);
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
        private bool Find(T key, out Node node, out Bucket<int> bucket) {
            Entry<T> entry = new Entry<T>(key);
            bucket = this.GetBucketInfo(entry);
            for (Node n = this.array[bucket.Index]; Assert.NotNull(n); n = n.Next) {
                if (entry.Equals(n.Key)) {
                    return this.IsAlive(n, bucket.Index, out node);
                }
            }
            node = null;
            return false;
        }
        public bool Get(T key, out U value) {
            bool num = this.Find(key, out Node node, out _);
            value = num ? node.Value : default;
            return num;
        }
        private Bucket<int> GetBucketInfo(Entry<T> entry) {
            int num = entry.GetHashCode() & int.MaxValue;
            return new Bucket<int>(num, num % this.array.Length);
        }
        public IEnumerator<KeyValuePair<T, U>> GetEnumerator() {
            for (int i = 0; i < this.array.Length; i++) {
                for (Node node = this.array[i]; Assert.NotNull(node); node = node.Next) {
                    if (node.Key.Reference.IsAlive) {
                        yield return new KeyValuePair<T, U>((T)node.Key.Reference.Target, node.Value);
                    }
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        /// <summary>Accesses the element with the specified key. If the key doesn't exists, the element is added to the container.</summary>
        public U GetOrAdd(T key, U value) {
            bool num = this.Find(key, out Node node, out Bucket<int> bucket);
            if (!num) {
                this.Add(key, value, bucket);
            }
            return num ? node.Value : value;
        }
        private void Initialize() {
            this.array = new Node[this.Capacity];
            this.Count = 0;
        }
        private void Initialize(WeakMap<T, U> map) {
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
            this.Capacity = capacity != 5 ? Numeric.NextPrime(capacity) : capacity;
        }
        private bool IsAlive(Node node, int index, out Node result) {
            bool num = node.Key.Reference.IsAlive;
            result = num ? node : null;
            return num || !this.Remove(node, index);
        }
        public IEnumerable<T> Keys() {
            for (int i = 0; i < this.array.Length; i++) {
                for (Node node = this.array[i]; Assert.NotNull(node); node = node.Next) {
                    if (node.Key.Reference.IsAlive) {
                        yield return (T)node.Key.Reference.Target;
                    }
                }
            }
        }
        public bool Remove(T key) {
            if (!this.Find(key, out Node node, out Bucket<int> bucket)) {
                return false;
            }
            return this.Remove(node, bucket.Index);
        }
        private bool Remove(Node node, int index) {
            this.array[index] = node.Next ?? null;
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
                        int num = current.Key.GetHashCode() % array.Length;
                        array[num] = new Node(current.Key, array[num], current.Value);
                        current = next;
                    }
                }
            }
            this.array = array;
        }
        /// <summary>Exchange contents of weak maps.</summary>
        public void Swap(WeakMap<T, U> map) {
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
        private KeyValuePair<T, U>[] Trim(KeyValuePair<T, U>[] source, int count) {
            if (count == this.Count) {
                return source;
            }
            KeyValuePair<T, U>[] array = new KeyValuePair<T, U>[count];
            Array.Copy(source, 0, array, 0, count);
            return array;
        }
        /// <summary>Copies the elements of the container to an array of KeyValuePairs.</summary>
        public KeyValuePair<T, U>[] ToArray() {
            KeyValuePair<T, U>[] array = new KeyValuePair<T, U>[this.Count];
            int num = 0;
            for (int i = 0; i < this.array.Length; i++) {
                for (Node node = this.array[i]; Assert.NotNull(node); node = node.Next) {
                    if (node.Key.Reference.IsAlive) {
                        array[num] = new KeyValuePair<T, U>((T)node.Key.Reference.Target, node.Value);
                        num++;
                    }
                }
            }
            return this.Trim(array, num);
        }
        public IEnumerable<U> Values() {
            for (int i = 0; i < this.array.Length; i++) {
                for (Node node = this.array[i]; Assert.NotNull(node); node = node.Next) {
                    if (node.Key.Reference.IsAlive) {
                        yield return node.Value;
                    }
                }
            }
        }
    }
}
