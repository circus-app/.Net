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
// A last-in-first-out (LIFO) contiguous container of T elements.


namespace Circus.Collections {
    /// <summary>Defines a last-in-first-out (LIFO) contiguous container of T elements.</summary>
    public interface IStack<T> : IContainer {
        /// <summary>Checks if the container contains the specified element. Out parameter returns the index of the element, otherwise -1.</summary>
        bool Contains(T value, out int index);
        /// <summary>Returns the element at the front of the container without removing it.</summary>
        T Peek();
        /// <summary>Removes and returns the element at the front of the container.</summary>
        bool Pop(out T value);
        /// <summary>Inserts the specified element to the container.</summary>
        bool Push(T value);
        /// <summary>Removes the specified element.</summary>
        void Remove(int index);
        /// <summary>Reverses the order of the elements.</summary>
        void Reverse();
    }
}
