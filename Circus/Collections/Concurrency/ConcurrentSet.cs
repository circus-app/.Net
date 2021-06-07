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
// An unordered thread-safe container of unique T elements.
//
// Same implementation as Set<T>. See Bag<T> for thread-safety details.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Circus.Text;
namespace Circus.Collections.Concurrency {
    [Serializable]
    /// <summary>Provides an unordered thread-safe container of unique T elements. Default capacity and offset is 5.</summary>
    public class ConcurrentSet<T> : ISet<T>, IEnumerable<T> {
        private class Node : Entry<T> {
            internal ulong Hash;
            internal volatile Node Next;
            internal Node(ulong hash, Node next, T value) : base(value) {
                this.Hash = hash;
                this.Next = next;
            }
        }        
        private IEqualityComparer<T> comparer;
        private int count;
        private bool flag;
        private volatile Threads<Node> list;
        public T this[T value] {
            get {
                return this.Contains(value) ? value : default;
            }
        }
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
        public ConcurrentSet() : this(5, 5) {
        }
        /// <summary>Constructs a container with the specified capacity and, the default offset.</summary>
        public ConcurrentSet(int capacity) : this(capacity, 5) {
        }
        /// <summary>Constructs a container with a copy of each of the elements in array.</summary>
        public ConcurrentSet(IEnumerable<T> array) : this(5, array) {
        }
        /// <summary>Constructs a container with the specified equality comparer.</summary>
        public ConcurrentSet(IEqualityComparer<T> comparer) : this(5, 5, Environment.ProcessorCount, comparer) {
        }
        /// <summary>Constructs a container with the specifed capacity and offset.</summary>
        public ConcurrentSet(int capacity, int offset) : this(capacity, offset, Environment.ProcessorCount, EqualityComparer<T>.Default) {
        }
        /// <summary>Constructs a container with the specifed capacity, offset and concurrency.</summary>
        public ConcurrentSet(int capacity, int offset, int concurrency) : this(capacity, offset, concurrency, EqualityComparer<T>.Default) { 
        }
        /// <summary>Constructs a container with the specified offset and a copy of each of the elements in array.</summary>
        public ConcurrentSet(int offset, IEnumerable<T> array) : this(offset, Environment.ProcessorCount, array) {
        }
        /// <summary>Constructs a container with the specified offset and concurrency and, a copy of each of the elements in array.</summary>
        public ConcurrentSet(int offset, int concurrency, IEnumerable<T> array) : this(offset, concurrency, array, EqualityComparer<T>.Default) {
        }
        /// <summary>Constructs a container with the specified capacity and equality comparer.</summary>
        public ConcurrentSet(int capacity, IEqualityComparer<T> comparer) : this(capacity, 5, comparer) {
        }
        /// <summary>Constructs a container with the specified capacity, concurrency and equality comparer.</summary>
        public ConcurrentSet(int capacity, int concurrency, IEqualityComparer<T> comparer) : this(capacity, 5, concurrency, comparer) {
        }
        /// <summary>Constructs a container with a copy of each of the elements in array and the specified equality comparer.</summary>
        public ConcurrentSet(IEnumerable<T> array, IEqualityComparer<T> comparer) : this(5, array, comparer) {
        }
        /// <summary>Constructs a container with the specified concurrency, a copy of each of the elements in array and the specified equality comparer.</summary>
        public ConcurrentSet(int concurrency, IEnumerable<T> array, IEqualityComparer<T> comparer) : this(5, concurrency, array, comparer) {
        }
        /// <summary>Constructs a container with the specified capacity, offset, concurrency and equality comparer.</summary>
        public ConcurrentSet(int capacity, int offset, int concurrency, IEqualityComparer<T> comparer) {
            this.comparer = comparer;
            this.Offset = offset;
            this.InitializeCore(capacity);
            this.Initialize(concurrency);
        }
        /// <summary>Constructs a container with the specified offset, conccurency, a copy of each of the elements in array and the specified equality comparer.</summary>
        public ConcurrentSet(int offset, int concurrency, IEnumerable<T> array, IEqualityComparer<T> comparer) {
            this.comparer = comparer;
            this.Offset = offset;
            this.InitializeCore(5);
            this.Initialize(concurrency, array);
        }
        public bool Add(T value) {
            int num = count;
            if (list.Enter(num, out object obj)) {
                if (this.Find(value, out _, out Bucket<ulong> bucket)) {
                    list.Exit(obj);
                    return false;
                }
                Volatile.Write(ref list.Entries[num], new Node(bucket.Hash, list.Entries[bucket.Index], value));
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
        private void CopyTo(Array array, int index, int count) {
            Array.Copy(list.Entries, 0, array, index, count);
        }
        public bool Contains(T value) {
            return this.Find(value, out _, out _);
        }
        private bool Find(T value, out Node node, out Bucket<ulong> bucket) {
            bucket = this.GetBucketInfo(value);
            for (Node n = Volatile.Read(ref list.Entries[bucket.Index]); n != null; n = n.Next) {
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
            return new Bucket<ulong>(num, (int)(num % (ulong)list.Entries.Length));
        }
        public IEnumerator<T> GetEnumerator() {
            Node[] array = list.Entries;
            for (int i = 0; i < array.Length; i++) {
                for (Node node = Volatile.Read(ref array[i]); node != null; node = node.Next) {
                    yield return node.Value;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        private void Initialize(int concurrency) {
            list = new Threads<Node>(concurrency, this.Capacity);
            count = 0;
        }
        private void Initialize(int concurrency, IEnumerable<T> array) {
            if (array is ConcurrentSet<T> set) {
                int num = set.list.Freeze();
                try {
                    list = new Threads<Node>(concurrency, set.count);
                    set.CopyTo(list.Entries, 0, set.count);
                    count = set.count;
                }
                finally {
                    set.list.Unfreeze(num);
                }
            }
            else {
                list = new Threads<Node>(concurrency, this.Capacity);
                foreach (T e in array) {
                    this.Add(e);
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
        public bool Remove(T value) {
            if (!this.Find(value, out Node node, out Bucket<ulong> bucket)) {
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
                        array[num] = new Node(current.Hash, array[num], current.Value);
                        current = next;
                    }
                }
            }
            list.Entries = array;
        }
        /// <summary>Exchange contents of sets.</summary>
        public void Swap(ConcurrentSet<T> set) {
            int num = list.Freeze();
            int num2 = set.list.Freeze();
            try {
                int num3 = count;
                Node[] array = this.Swap(list.Entries, list.Entries.Length);
                list.Entries = this.Swap(set.list.Entries, set.list.Entries.Length);
                count = set.count;
                set.list.Entries = array;
                set.count = num3;
            }
            finally {
                list.Unfreeze(num);
                set.list.Unfreeze(num2);
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
        /// <summary>Copies the elements of the container to an array.</summary>
        public T[] ToArray() {
            T[] array = new T[count];
            int num = 0;
            for (int i = 0; i < list.Entries.Length; i++) {
                for (Node node = Volatile.Read(ref list.Entries[i]); node != null; node = node.Next) {
                    array[num] = node.Value;
                    num++;
                }
            }
            return array;
        }
    }
}
