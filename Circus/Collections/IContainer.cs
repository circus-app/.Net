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
// A container with initial capacity and offset.


namespace Circus.Collections {
    /// <summary>Defines a container with initial capacity and offset.</summary>
    public interface IContainer {
        /// <summary>Returns the default size of the container.</summary>
        int Capacity { get; }
        /// <summary>Returns the number of elements.</summary>
        int Count { get; }
        /// <summary>Checks whether the container is empty.</summary>
        bool Empty { get; }
        /// <summary>Returns the offset that will be applied when reallocating.</summary>
        int Offset { get; }
        /// <summary>Returns the size of the container.</summary>
        int Size { get; }
        /// <summary>Clears the content.</summary>
        void Clear();
        /// <summary>Clears the content and optionally resizes the container to the capacity and offset values.</summary>
        void Clear(bool trim);
        /// <summary>Reallocates to the specified size.</summary>
        void Reserve(int size);
        /// <summary>Resizes to the specified size.</summary>
        void Resize(int size);
        /// <summary>Shrinks the container to the number of elements.</summary>
        void Trim();
    }
}
