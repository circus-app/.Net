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
// An observable pail of T elements.
//
// Same implementation as Pail<T>.


#pragma warning disable IDE0002

using System;
using System.Collections;
namespace Circus.Collections.Observable {
    /// <summary>Provides an observable basket of T elements filtered by a predicate.</summary>
    [Serializable]
    public class ObservablePail<T> : Basket<T> {
        /// <summary>A predicate of U type that outputs an instance of T.</summary>
        public delegate bool Predicate<U>(U source, out T result);
        private ObservablePail() {
        }
        private ObservablePail(ObservablePail<T> array) : base(array) {
        }
        private ObservablePail(int capacity, int offset) : base(capacity, offset) {
        }
        /// <summary>Returns an observable pail with a copy of each of the elements in array, in the same order.</summary>
        public static ObservablePail<T> Create(ObservablePail<T> array) {
            return new ObservablePail<T>(array);
        }
        /// <summary>Returns an observable pail filled with elements of array, filtered by the specified system predicate.</summary>
        public static ObservablePail<T> Create(IEnumerable array, System.Predicate<T> predicate) {
            return ObservablePail<T>.Create(5, array, predicate);
        }
        /// <summary>Returns an observable pail filled with elements created by the specified pail predicate.</summary>
        public static ObservablePail<T> Create<U>(IEnumerable array, Predicate<U> predicate) {
            return ObservablePail<T>.Create<U>(5, array, predicate);
        }
        /// <summary>Returns an observable pail with the specified offset, filled with elements of array, filtered by the provided system predicate.</summary>
        public static ObservablePail<T> Create(int offset, IEnumerable array, System.Predicate<T> predicate) {
            ObservablePail<T> result = ObservablePail<T>.Initialize(offset, array);
            foreach (T e in array) {
                if (predicate(e)) {
                    result.Add(e);
                }
            }
            result.Trim();
            return result;
        }
        /// <summary>Returns an observable pail with the specified offset, filled with elements created by the provided pail predicate.</summary>
        public static ObservablePail<T> Create<U>(int offset, IEnumerable array, Predicate<U> predicate) {
            ObservablePail<T> result = ObservablePail<T>.Initialize(offset, array);
            foreach (U e in array) {
                if (predicate(e, out T r)) {
                    result.Add(r);
                }
            }
            result.Trim();
            return result;
        }
        private static ObservablePail<T> Initialize(int offset, IEnumerable array) {
            return new ObservablePail<T>(array is IContainer collection ? collection.Count : 5, offset);
        }
    }
}