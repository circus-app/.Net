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
// Event data for a collection filtering execution.


using System;
namespace Circus.Wpf.Data {
    /// <summary>Provides event data for a collection filtering execution.</summary>
    public class FilterCompletedEventArgs : EventArgs {
        /// <summary>Determines if the filter returned an empty result.</summary>
        public bool IsEmpty { get; private set; }
        /// <summary>Determines if the the collection is filtered.</summary>
        public bool IsFiltered { get; private set; }
        private FilterCompletedEventArgs() { 
        }
        /// <summary>Constructs a filter completed event args with the specified filter status and a flag that determines if the result is empty.</summary>
        public FilterCompletedEventArgs(bool filtered, bool empty) {
            this.IsFiltered = filtered;
            this.IsEmpty = empty;
        }
    }
}
