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
// A component to keep track of toggled documents.
//
// The class is marked as public only to match DocumentTrayItemsSouce
// accessibility.
//
// IsDrop is called on document drag to determine if the desired position
// is valid depending on its toggled state. A toggled document can be moved 
// anywhere. An untoggled document cannot move beyond the Next index.
//
// Update is used to determine if the document should be toggled to true 
// once moved. This only happens if the document is toggled and moved 
// within the toggled area. 
//
// IsMoved is called on document toggled to determine if the document
// should be moved to the toggled area. If true, the function outputs the
// target position.
//
// Update is called on document close or float to update the stack position if the
// document was toggled.


using Circus.Runtime;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a component to keep track of toggled documents.</summary>
    public sealed class DocumentTrayStackInfo {
        internal bool Empty => this.Next == 0;
        internal int Next { get; private set; }
        internal DocumentTrayStackInfo() {
            this.Reset();
        }
        internal bool IsDrop(int position, bool toggled, out bool update) {
            update = toggled && position < this.Next;
            if (toggled && !update) {
                this.Pop();
            }
            return toggled || position >= this.Next;
        }
        internal bool IsMoved(int index, bool toggled, out int position) {
            if (toggled && Allocator.Assign(this.Next, out position)) {
                this.Push();
            }
            else {
                this.Pop();
                position = this.Next;
            }
            return position != index;
        }
        private void Pop() {
            this.Next--;
        }
        private void Push() {
            this.Next++;
        }
        internal void Reset() {
            this.Reset(0);
        }
        internal void Reset(int value) {

            // Reset stack info to the provided value. 
            this.Next = value;
        }
        internal void Update(bool toggled) {
            if (toggled) {
                this.Pop();
            }
        }
    }
}
