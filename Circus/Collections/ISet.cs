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


namespace Circus.Collections {
    /// <summary>Defines an unordered container of unique T elements.</summary>
    public interface ISet<T> : IContainer {
        /// <summary>Returns the specified element.</summary>
        T this[T value] { get; }
        /// <summary>Inserts the specified element to the container. Returns true if insertion succeded.</summary>
        bool Add(T value);
        /// <summary>Checks if the container contains the specified element.</summary>
        bool Contains(T value);
        /// <summary>Removes the specified element. Returns true if removal succeded.</summary>
        bool Remove(T value);
    }
}
