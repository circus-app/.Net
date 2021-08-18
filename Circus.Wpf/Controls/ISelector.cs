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
// An element that provides SelectorView support.


namespace Circus.Wpf.Controls {
    /// <summary>Defines an element that provides SelectorView support.</summary>
    public interface ISelector {
        /// <summary>Returns the data context of the element.</summary>
        object DataContext { get; }
        /// <summary>Returns the header of the element.</summary>
        object Header { get; }
        /// <summary>Returns the icon of the element.</summary>
        object Icon { get; }
        /// <summary>Closes the element.</summary>
        void Close();
    }
}
