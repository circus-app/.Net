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
// A vector of T elements filtered by a predicate.
//
// This is what we could call a 'Projection' container.
//
// The idea was to have a vector that is initialized with an enumerable, 
// filtered by a predicate. 
//
// A typical scenario would be using Reflection to return a list of methods, 
// but having to filter the result based on different criterias. In that case, 
// the common approach would be to initialize a list with the size of the result, 
// filter within a loop and optionally trim at the end of the process to avoid 
// memory waist.
//
// Pail provides all these steps in static Create methods, without having to 
// allocate, encapsulate in a foreach statement and trimming at the end.
//
// Overloaded Create methods provide a generic U that is the type of the 
// enumerable. This allows the source enumerable to be of a different type than T.
//
// If we go back to the above example, what if we would like to filter methods 
// names that start with 'A' and add this property to another type that will be 
// the element of our container ? Well, this what T and U stand for.
//
// Pail uses two kinds of predicates. A system predicate for cases where T and U 
// are of the same type, and, a Pail.Predicate which is a delegate that takes U as 
// source, and returns T in an output parameter (T is an output parameter since
// predicates always return bool).
//
// Pail.Predicate is useful for filtering or creating instances of T in an external
// method like this:
//
// List<int> list = new List<int>();
// Pail<SomeType> pail = Pail<SomeType>.Create<MethodInfo>(list.GetType().GetMethods(), this.GetMember);
//
// private bool GetMember(MethodInfo i, out SomeType result) {
//      if (i.Name.StartWith("A")) {
//          result = new SomeType(i.Name);
//          return true;
//      }
//      return false;
// }
//
// In this example, pail contains a sequence of SomeType each created with a list 
// member which name starts with 'A'.
//
// A more common use can be achieved using the system predicate:
// 
// List<int> list = new List<int>(5) { 1, 2, 3, 4, 5 };
// Pail<int> pail = Pail<int>.Create(list, i => i > 1);
//
// In this case, pail contains a sequence of int greater than 1.
//
// For consistency, pail does not provide public constructors. Use the overloaded
// static Create methods to create instances.
//
// Since it derives from Vector, casting is not necessary in inherited Swap().


#pragma warning disable IDE0002

using System;
using System.Collections;
namespace Circus.Collections {
    /// <summary>Provides a vector of T elements filtered by a predicate.</summary>
    [Serializable]
    public class Pail<T> : Vector<T> {
        /// <summary>A predicate of U type that outputs an instance of T.</summary>
        public delegate bool Predicate<U>(U source, out T result);
        private Pail() { 
        }
        private Pail(Pail<T> array) : base(array) {
        }
        private Pail(int capacity, int offset) : base(capacity, offset) {
        }
        /// <summary>Returns a pail with a copy of each of the elements in array, in the same order.</summary>
        public static Pail<T> Create(Pail<T> array) {
            return new Pail<T>(array);
        }
        /// <summary>Returns a pail filled with elements of array, filtered by the specified system predicate.</summary>
        public static Pail<T> Create(IEnumerable array, System.Predicate<T> predicate) {
            return Pail<T>.Create(5, array, predicate);
        }
        /// <summary>Returns a pail filled with elements created by the specified pail predicate.</summary>
        public static Pail<T> Create<U>(IEnumerable array, Predicate<U> predicate) {
            return Pail<T>.Create<U>(5, array, predicate);
        }
        /// <summary>Returns a pail with the specified offset, filled with elements of array, filtered by the provided system predicate.</summary>
        public static Pail<T> Create(int offset, IEnumerable array, System.Predicate<T> predicate) {
            Pail<T> result = Pail<T>.Initialize(offset, array);
            foreach (T e in array) {
                if (predicate(e)) {
                    result.Add(e);
                }
            }
            result.Trim();
            return result;
        }
        /// <summary>Returns a pail with the specified offset, filled with elements created by the provided pail predicate.</summary>
        public static Pail<T> Create<U>(int offset, IEnumerable array, Predicate<U> predicate) {
            Pail<T> result = Pail<T>.Initialize(offset, array);
            foreach (U e in array) {
                if (predicate(e, out T r)) {
                    result.Add(r);
                }
            }
            result.Trim();
            return result;
        }
        private static Pail<T> Initialize(int offset, IEnumerable array) { 
            return new Pail<T>(array is ICollection collection ? collection.Count : 5, offset);
        }
    }
}
