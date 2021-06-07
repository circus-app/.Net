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
// A lightweight object to store two values of type T.
//
// This is more or less the same as a Tuple object but providing a single 
// type T since it is mostly used to store values of the same kind 
// (i.e. indexes, height/width, etc.).
//
// The object is a class instead of a struct to allow inheritance 
// (see Progress for an example of use).


#pragma warning disable IDE0002

using System;
using System.Collections.Generic;
namespace Circus {
	/// <summary>Provides a lightweight object to store two values of type T.</summary>
	[Serializable]
	public class Duple<T> : IComparable<Duple<T>>, IComparable, IEquatable<Duple<T>> {
		/// <summary>Returns the value of the specified element. First is 0, second is 1. Otherwise default.</summary>
		public T this[int index] => index == 0 ? this.First : index == 1 ? this.Second : default;
		public static Duple<T> Empty => new Duple<T>(default, default);
		/// <summary>Gets or sets the value of the first component.</summary>
		public T First { get; set; }
		/// <summary>Gets or sets the value of the second component.</summary>
		public T Second { get; set; }
		private Duple() { 
		}
		/// <summary>Constructs a duple with the specified first and second values.</summary>
		public Duple(T first, T second) {
			this.First = first;
			this.Second = second;
		}
		private static int Combine(int x, int y) {
			uint num = (uint)((x << 5) | (int)((uint)x >> 27));
			return ((int)num + x) ^ y;
		}
		public int CompareTo(Duple<T> other) {
			int num = Comparer<T>.Default.Compare(this.First, other.First);
			return num != 0 ? num : Comparer<T>.Default.Compare(this.Second, other.Second);
		}
		public int CompareTo(object obj) {
			return this.CompareTo((Duple<T>)obj);
		}
		public bool Equals(Duple<T> other) {
			return EqualityComparer<T>.Default.Equals(this.First, other.First) && EqualityComparer<T>.Default.Equals(this.Second, other.Second);
		}
		public override bool Equals(object obj) {
			return this.Equals((Duple<T>)obj);
		}
		public override int GetHashCode() {
			return Duple<T>.Combine(EqualityComparer<T>.Default.GetHashCode(this.First), EqualityComparer<T>.Default.GetHashCode(this.Second));
		}
		public override string ToString() {
			return string.Format("{0}, {1}", Assert.NotNull(this.First) ? this.First.ToString() : "null", Assert.NotNull(this.Second) ? this.Second.ToString() : "null");
		}
	}
}
