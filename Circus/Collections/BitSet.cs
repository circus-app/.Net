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
// A container of bits. 
//
// The class emulates an array of bool elements, but optimized for space 
// allocation. Each element occupies only one bit.

// Each bit position can be accessed individually. For example, for a given 
// BitSet named Foo, the expression Foo[3] accesses its fourth bit, just 
// like a regular array.
//
// Dynamic resizing is available using Add() methods. This is intended for 
// cases where the container is inherited from a base class, permitting the
// derived type to implement its own set of bits.
//
// New bits are inserted at the end of the container which implies to be 
// extremely precautious using inheritance since the derived type can 
// overwrite positions used by the base class.  
//
// Index accessor do not perform any bounds checking. Use At() or SetAt() 
// instead if bounds safety is required.
//
// The default size is 4.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Circus.Runtime;
namespace Circus.Collections {
    /// <summary>Provides a container of bits.</summary>
    public sealed class BitSet : IEnumerable<bool> {
        private int[] array;
        /// <summary>Gets or sets the specified element.</summary>
        public bool this[int index] { get => this.Get(index); set => this.Set(index, value); }
        /// <summary>Determines if any bit is set.</summary>
        public bool Any => this.Find();
        /// <summary>Determines if no bit is set.</summary>
        public bool None => !this.Find();
        /// <summary>Returns the size of the container.</summary>
        public int Size { get; private set; }
        /// <summary>Constructs a container with the default size.</summary>
        public BitSet() : this(4) { 
        }
        /// <summary>Constructs a container with a copy of each of the elements in set, in the same order.</summary>
        public BitSet(BitSet set) {
            if (Allocator.Assign(set.array.Length, out int num)) {
                this.array = new int[num];
                Array.Copy(set.array, this.array, num);
            }
        }
        /// <summary>Constructs a container with the specified size.</summary>
        public BitSet(int size) : this(size, false) { 
        }
        /// <summary>Constructs a container with the specified array of bit values.</summary>
        public BitSet(params bool[] array) {
            if (Allocator.Assign(array.Length, out int num)) {
                this.Initialize(num);
                this.Fill(0, num, array);
            }
        }
        /// <summary>Constructs a container with the specified size and filled the provided value.</summary>
        public BitSet(int size, bool value) {
            this.Initialize(size);
            this.Fill(0, value);
        }
        /// <summary>Inserts an unset bit to the container.</summary>
        public void Add() {
            this.Add(1);
        }
        /// <summary>Inserts the specified number of unset bits values to the container.</summary>
        public void Add(int count) {
            this.Add(count, false);
        }
        /// <summary>Inserts the specified array of bit values to the container.</summary>
        public void Add(params bool[] array) {
            if (Allocator.Assign(array.Length, out int num) && Allocator.Assign(this.Insert(num), out int index)) {
                this.Fill(index, num, array);
            }
        }
        /// <summary>Inserts the specified number of bits with the provided value to the container.</summary>
        public void Add(int count, bool value) {
            if (Allocator.Assign(this.Insert(count), out int num)) {
                this.Fill(num, value);
            }
        }
        /// <summary>Returns the specified element with bounds checking.</summary>
        public bool At(int index, out bool value) {
            return (index < this.Size && Allocator.Assign(this.Get(index), out value)) || !Allocator.Assign(false, out value); 
        }
        /// <summary>Returns iterator to beginning with the specified number of elements.</summary>
        public IEnumerable<bool> Begin(int count) {
            for (int i = 0; i < count; i++) {
                yield return this.Get(i);
            }
        }
        /// <summary>Returns iterator to end.</summary>
        public IEnumerable<bool> End() {
            return this.End(this.Size);
        }
        /// <summary>Returns iterator to end with the specified number of elements.</summary>
        public IEnumerable<bool> End(int count) {
            if (Allocator.Assign(this.Size - 1, out int num)) {
                for (int i = num; i >= num - count; i--) {
                    yield return this.Get(i);
                }
            }
        }
        private void Fill(int index, int count, params bool[] array) {
            for (int i = index; i < count; i++) {
                if (array[i]) {
                    this.array[i / 32] |= 1 << i % 32;
                }
            }
        }
        private void Fill(int index, bool value) {
            if (Allocator.Assign(value ? 1 : 0, out int bit)) {
                for (int i = index; i < this.array.Length; i++) {
                    this.array[i] = bit;
                }
            }
        }
        private bool Find() {
            foreach (bool value in this) {
                if (value) {
                    return true;
                }
            }
            return false;
        }
        private int Insert(int count) {
            if (Allocator.Assign(this.Size, out int num) && Allocator.Assign(this.Size + count, out int size) && size > this.array.Length && Allocator.Assign(new int[this.GetSize(size)], out int[] array)) {
                Array.Copy(this.array, array, this.array.Length);
                this.Initialize(array, size);
            }
            return num;
        }
        private void Initialize(int size) {
            this.Initialize(new int[this.GetSize(size)], size);
        }
        private void Initialize(int[] array, int size) {
            this.array = array;
            this.Size = size;
        }
        private bool Get(int index) {
            return (this.array[index / 32] & (1 << index % 32)) != 0;
        }
        private int GetSize(int size) {
            return (size - 1) / 32 + 1;
        }
        public IEnumerator<bool> GetEnumerator() {
            return this.Begin(this.Size).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        private void Set(int index, bool value) {
            if (value) {
                this.array[index / 32] |= 1 << index % 32;
            }
            else {
                this.array[index / 32] &= ~(1 << index % 32);
            }
        }
        /// <summary>Sets the value of the element at the specified position with bounds checking.</summary>
        public bool SetAt(int index, bool value) {
            if (Allocator.Assign(index < this.Size, out bool num) && num) {
                this.Set(index, value);
            }
            return num;
        }
        /// <summary>Exchange contents of BitSets.</summary>
        public void Swap(BitSet set) {
            if (Allocator.Assign(this.Size, out int num) && Allocator.Assign(this.Swap(this.array, num), out int[] array) && Allocator.Assign(this.Swap(set.array, set.Size), out this.array) && Allocator.Assign(array, out set.array)) {
                this.Size = set.Size;
                set.Size = num;
            }
        }
        private int[] Swap(Array source, int size) {
            if (Allocator.Assign(new int[size], out int[] array)) {
                Array.Copy(source, 0, array, 0, size);
            }            
            return array;
        }
        /// <summary>Copies the elements of the container to an array.</summary>
        public bool[] ToArray() {
            if (Allocator.Assign(this.Size, out int num) & Allocator.Assign(new bool[num], out bool[] array)) {
                for (int i = 0; i < num; i++) {
                    array[i] = this.Get(i);
                }
            }
            return array;
        }
        public override string ToString() {
            if (Allocator.Assign(this.Size, out int num) & Allocator.Assign(new StringBuilder(num), out StringBuilder array)) {
                for (int i = 0; i < num; i++) {
                    array.AppendFormat("{0}, ", this.Get(i));
                }
            }
            return array.Remove(array.Length - 2, 2).ToString();
        }
    }
}
