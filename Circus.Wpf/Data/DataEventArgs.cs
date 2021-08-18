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
// Event data for a data source event.


using System;
namespace Circus.Wpf.Data {
    /// <summary>Provides data for a data source event.</summary>
    public sealed class DataEventArgs : EventArgs {
        /// <summary>Returns an empty event args.</summary>
        public static new DataEventArgs Empty => new DataEventArgs();
        /// <summary>Returns the value of the DataEventArgs.</summary>
        public object Value { get; private set; }
        private DataEventArgs() { 
        }
        /// <summary>Constructs a DataEventArgs with the specified value.</summary>
        public DataEventArgs(object value) {
            this.Value = value;
        }
    }
}
