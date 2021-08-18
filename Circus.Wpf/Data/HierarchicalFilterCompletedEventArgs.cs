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
// Event data for a hierarchical collection filtering execution.


namespace Circus.Wpf.Data {
    /// <summary>Provides event data for a hierarchical collection filtering execution.</summary>
    public sealed class HierarchicalFilterCompletedEventArgs : FilterCompletedEventArgs {
        /// <summary>Returns the number of excluded items.</summary>
        public int Count { get; private set; }
        /// <summary>Constructs a hierarchical filter completed event args with the specified number of resulting items, a filter status and a flag that determines if the result is empty.</summary>
        public HierarchicalFilterCompletedEventArgs(int count, bool filtered, bool empty) : base(filtered, empty) {
            this.Count = count;
        }
    }
}
