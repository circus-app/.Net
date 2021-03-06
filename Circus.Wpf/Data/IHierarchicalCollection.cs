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
// A hierarchical wrapper around a container of type T that provides filtering
// event handling.


using System;
namespace Circus.Wpf.Data {
    /// <summary>Defines a hierarchical wrapper around a container of type T that provides filtering event handling.</summary>
    public interface IHierarchicalCollection : IDataCollection {
        /// <summary>Occurs when the filtering completed.</summary>
        event HierarchicalFilterCompletedEventHandler FilterCompleted;
    }
}
