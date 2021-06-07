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
// A contiguous container of T elements with index accessors.


namespace Circus.Collections {
    /// <summary>Defines a contiguous container of T elements with index accessors.</summary>
    public interface IVector<T> : IContainer {
        /// <summary>Gets or sets the specified element.</summary>
        T this[int index] { get; }
        /// <summary>Gets or sets the last element.</summary>
        T Back { get; }
        /// <summary>Gets or sets the first element.</summary>
        T Front { get; }
        /// <summary>Inserts the specified element to the container and returns its position.</summary>
        int Add(T value);
        /// <summary>Checks if the container contains the specified element. Out parameter returns the index of the element, otherwise -1.</summary>
        bool Contains(T value, out int index);
        /// <summary>Inserts element at the specified position.</summary>
        void Insert(int index, T value);
        /// <summary>Removes the specified element.</summary>
        void Remove(int index);
        /// <summary>Reverses the order of the elements.</summary>
        void Reverse();
    }
}
