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
// An unordered associative thread-safe container of T keys and U values.
//
// Same implementation as Map<T>. See Bag<T> for thread-safety details.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Circus.Text;
namespace Circus.Collections.Concurrency {
    /// <summary>Provides an unordered associative thread-safe container of T keys and U values. Default capacity and offset is 5.</summary>
    [Serializable]
    public class ConcurrentMap<T, U> : IMap<T, U>, IEnumerable<KeyValuePair<T, U>> {
        private class Node : Entry<U> {
            internal ulong Hash;
            internal T Key;
            internal volatile Node Next;
            internal Node(ulong hash, T key, Node next, U value) : base(value) {
                this.Hash = hash;
                this.Key = key;
                this.Next = next;
            }
        }
        private IEqualityComparer<T> comparer;
        private int count;
        private bool flag;
        private volatile Threads<Node> list;
        public U this[T key] { get => this.Get(key, out U value) ? value : default; set => this.Add(key, value, false); }
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
        public ConcurrentMap() : this(5) {
        }
        /// <summary>Constructs a container with the specified capacity and, the default offset.</summary>
        public ConcurrentMap(int capacity) : this(capacity, 5) {
        }
        /// <summary>Constructs a container with a copy of each of the elements in array.</summary>
        public ConcurrentMap(IEnumerable<KeyValuePair<T, U>> array) : this(5, array) {
        }
        /// <summary>Constructs a container with the specified equality comparer.</summary>
        public ConcurrentMap(IEqualityComparer<T> comparer) : this(5, comparer) {
        }
        /// <summary>Constructs a container with a copy of the elements in map.</summary>
        public ConcurrentMap(ConcurrentMap<T, U> map) : this(map.Offset, map) {
        }
        /// <summary>Constructs a container with the specifed capacity and offset.</summary>
        public ConcurrentMap(int capacity, int offset) : this(capacity, offset, Environment.ProcessorCount) {
        }
        /// <summary>Constructs a container with the specifed capacity, offset and concurrency.</summary>
        public ConcurrentMap(int capacity, int offset, int concurrency) : this(capacity, offset, concurrency, EqualityComparer<T>.Default) {
        }
        /// <summary>Constructs a container with the specified offset and a copy of each of the elements in array.</summary>
        public ConcurrentMap(int offset, IEnumerable<KeyValuePair<T, U>> array) : this(offset, Environment.ProcessorCount, array) {
        }
        /// <summary>Constructs a container with the specified offset and concurrency and, a copy of each of the elements in array.</summary>
        public ConcurrentMap(int offset, int concurrency, IEnumerable<KeyValuePair<T, U>> array) : this(offset, concurrency, array, EqualityComparer<T>.Default) {
        }
        /// <summary>Constructs a container with the specified capacity and equality comparer.</summary>
        public ConcurrentMap(int capacity, IEqualityComparer<T> comparer) : this(capacity, Environment.ProcessorCount, comparer) {
        }
        /// <summary>Constructs a container with the specified capacity, concurrency and equality comparer.</summary>
        public ConcurrentMap(int capacity, int concurrency, IEqualityComparer<T> comparer) : this(capacity, concurrency, 5, comparer) {
        }
        public ConcurrentMap(int offset, ConcurrentMap<T, U> map) {
            this.Offset = offset;
            this.InitializeCore(5);
            this.Initialize(Environment.ProcessorCount, map);
        }
        /// <summary>Constructs a container with the specified offset and a copy of the elements in map.</summary>
        public ConcurrentMap(int offset, int concurrency, Map<T, U> map) {
            this.Offset = offset;
            this.InitializeCore(5);
            this.Initialize(concurrency, map);
        }
        /// <summary>Constructs a container with a copy of each of the elements in array and the specified equality comparer.</summary>
        public ConcurrentMap(IEnumerable<KeyValuePair<T, U>> array, IEqualityComparer<T> comparer) : this(Environment.ProcessorCount, array, comparer) {
        }
        /// <summary>Constructs a container with the specified concurrency, a copy of each of the elements in array and the specified equality comparer.</summary>
        public ConcurrentMap(int concurrency, IEnumerable<KeyValuePair<T, U>> array, IEqualityComparer<T> comparer) : this(5, concurrency, array, comparer) {
        }
        /// <summary>Constructs a container with the specified capacity, offset, concurrency and equality comparer.</summary>
        public ConcurrentMap(int capacity, int offset, int concurrency, IEqualityComparer<T> comparer) {
            this.comparer = comparer;
            this.Offset = offset;
            this.InitializeCore(capacity);
            this.Initialize(concurrency);
        }
        /// <summary>Constructs a container with the specified offset, conccurency, a copy of each of the elements in array and the specified equality comparer.</summary>
        public ConcurrentMap(int offset, int concurrency, IEnumerable<KeyValuePair<T, U>> array, IEqualityComparer<T> comparer) {
            this.comparer = comparer;
            this.Offset = offset;
            this.InitializeCore(5);
            this.Initialize(concurrency, array);
        }
        public bool Add(T key, U value) {
            return this.Add(key, value, false);
        }
        private bool Add(T key, U value, bool update) {
            int num = count;
            if (list.Enter(num, out object obj)) {
                bool f = this.Find(key, out Node node, out Bucket<ulong> bucket);
                if (f) {
                    if (update) {
                        node.Value = value;
                    }
                    list.Exit(obj);
                    return update;
                }
                Volatile.Write(ref list.Entries[num], new Node(bucket.Hash, key, list.Entries[bucket.Index], value));
                count++;
                list.Exit(obj);
            }
            if (count == list.Entries.Length) {
                int num2 = list.Freeze();
                try {
                    this.ResizeCore(list.Entries.Length + this.Offset);
                }
                finally {
                    list.Unfreeze(num2);
                }
            }
            return true;
        }
        private bool Add(T key, U value, Bucket<ulong> bucket) {
            int num = count;
            if (list.Enter(num, out object obj)) {
                Volatile.Write(ref list.Entries[num], new Node(bucket.Hash, key, list.Entries[bucket.Index], value));
                count++;
                list.Exit(obj);
            }
            if (count == list.Entries.Length) {
                int num2 = list.Freeze();
                try {
                    this.ResizeCore(list.Entries.Length + this.Offset);
                }
                finally {
                    list.Unfreeze(num2);
                }
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
            int num = list.Freeze();
            try {
                if (count > 0) {
                    if (trim) {
                        list.Entries = new Node[this.Capacity];
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
        public bool Contains(T key) {
            return this.Find(key, out _, out _);
        }
        private void CopyTo(Array array, int index, int count) {
            Array.Copy(list.Entries, 0, array, index, count);
        }
        private bool Find(T key, out Node node, out Bucket<ulong> bucket) {
            bucket = this.GetBucketInfo(key);
            for (Node n = Volatile.Read(ref list.Entries[bucket.Index]); n != null; n = n.Next) {
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
            return new Bucket<ulong>(num, (int)(num % (ulong)list.Entries.Length));
        }
        public IEnumerator<KeyValuePair<T, U>> GetEnumerator() {
            Node[] array = list.Entries;
            for (int i = 0; i < array.Length; i++) {
                for (Node node = Volatile.Read(ref array[i]); node != null; node = node.Next) {
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
        private void Initialize(int concurrency) {
            list = new Threads<Node>(concurrency, this.Capacity);
            count = 0;
        }
        private void Initialize(int concurrency, ConcurrentMap<T, U> map) {
            int num = map.list.Freeze();
            try {
                list = new Threads<Node>(concurrency, map.count);
                map.CopyTo(list.Entries, 0, map.count);
                count = map.count;
            }
            finally {
                map.list.Unfreeze(num);
            }
        }
        private void Initialize(int concurrency, IEnumerable<KeyValuePair<T, U>> array) {
            list = new Threads<Node>(concurrency, this.Capacity);
            foreach (KeyValuePair<T, U> p in array) {
                this.Add(p.Key, p.Value);
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
        public IEnumerable<T> Keys() {
            Node[] array = list.Entries;
            for (int i = 0; i < array.Length; i++) {
                for (Node node = Volatile.Read(ref array[i]); node != null; node = node.Next) {
                    yield return node.Key;
                }
            }
        }
        public bool Remove(T key) {
            if (!this.Find(key, out Node node, out Bucket<ulong> bucket)) {
                return false;
            }
            Volatile.Write(ref list.Entries[bucket.Index], node.Next ?? null);
            count--;
            return true;
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
                if (size < count) {
                    return;
                }
                this.ResizeCore(size);
            }
            finally {
                list.Unfreeze(num);
            }
        }
        private void ResizeCore(int size) {
            size = Numeric.NextPrime(size);
            Node[] array = new Node[size];
            for (int i = 0; i < list.Entries.Length; i++) {
                Node current = list.Entries[i];
                checked {
                    while (current != null) {
                        Node next = current.Next;
                        int num = (int)(current.Hash % (ulong)array.Length);
                        array[num] = new Node(current.Hash, current.Key, array[num], current.Value);
                        current = next;
                    }
                }
            }
            list.Entries = array;
        }
        /// <summary>Exchange contents of maps.</summary>
        public void Swap(ConcurrentMap<T, U> map) {
            int num = list.Freeze();
            int num2 = map.list.Freeze();
            try {
                int num3 = count;
                Node[] array = this.Swap(list.Entries, list.Entries.Length);
                list.Entries = this.Swap(map.list.Entries, map.list.Entries.Length);
                count = map.count;
                map.list.Entries = array;
                map.count = num3;
            }
            finally {
                list.Unfreeze(num);
                map.list.Unfreeze(num2);
            }
        }
        private Node[] Swap(Array source, int size) {
            Node[] array = new Node[size];
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
        /// <summary>Copies the elements of the container to an array of KeyValuePairs.</summary>
        public KeyValuePair<T, U>[] ToArray() {
            KeyValuePair<T, U>[] array = new KeyValuePair<T, U>[this.Count];
            int num = 0;
            for (int i = 0; i < list.Entries.Length; i++) {
                for (Node node = Volatile.Read(ref list.Entries[i]); node != null; node = node.Next) {
                    array[num] = new KeyValuePair<T, U>(node.Key, node.Value);
                    num++;
                }
            }
            return array;
        }
        public IEnumerable<U> Values() {
            Node[] array = list.Entries;
            for (int i = 0; i < array.Length; i++) {
                for (Node node = Volatile.Read(ref array[i]); node != null; node = node.Next) {
                    yield return node.Value;
                }
            }
        }
    }
}
