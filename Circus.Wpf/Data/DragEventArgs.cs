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
// Event data for a drag event.


using System;
namespace Circus.Wpf.Data {
    /// <summary>Provides data for a drag event.</summary>
    public sealed class DragEventArgs : EventArgs {
        /// <summary>Returns an empty event args.</summary>
        public static new DragEventArgs Empty => new DragEventArgs();
        /// <summary>Returns the object that started the drag operation.</summary>
        public object Source { get; private set; }
        /// <summary>Returns the object that is the target of the drag operation.</summary>
        public object Target { get; private set; }
        private DragEventArgs() {
        }
        /// <summary>Constructs a DragEventArgs with the specified source and target.</summary>
        public DragEventArgs(object source, object target) {
            this.Source = source;
            this.Target = target;
        }
    }
}
