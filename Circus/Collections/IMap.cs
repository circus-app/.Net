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


using System.Collections.Generic;
namespace Circus.Collections {
    /// <summary>Defines an unordered associative container of T keys and U values.</summary>
    public interface IMap<T, U> : IContainer {
        /// <summary>Gets or sets the value associated with the specified key.</summary>
        U this[T key] { get; set; }
        /// <summary>Inserts the specified element to the container. Returns false if the key already exists.</summary>
        bool Add(T key, U value);
        /// <summary>Checks if the container contains the specified element.</summary>
        bool Contains(T key);
        /// <summary>Accesses the element with the specified key. Return true if the key exists.</summary>
        bool Get(T key, out U value);
        /// <summary>Returns iterator of keys.</summary>
        IEnumerable<T> Keys();
        /// <summary>Removes the specified element. Returns true if removal succeded.</summary>
        bool Remove(T key);
        /// <summary>Returns iterator of values.</summary>
        IEnumerable<U> Values();
    }
}
