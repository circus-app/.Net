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
// A wrapper around a container of type T that provides a count of the current 
// enumeration and a filter state.


using System.Collections;
using System.Collections.Specialized;
namespace Circus.Wpf.Data {
    /// <summary>Defines a wrapper around a container of type T that provides a count of the current enumeration and a filter state.</summary>
    public interface IDataCollection : IEnumerable, INotifyCollectionChanged {
        /// <summary>Returns the number of items resulting from the current enumeration.</summary>
        int Count { get; }
        /// <summary>Determines if the collection is filtered.</summary>
        bool IsFiltered { get; }
    }
}
