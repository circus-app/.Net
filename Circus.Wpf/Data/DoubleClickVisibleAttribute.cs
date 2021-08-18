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
// An attribute that is applied to the class definition which specifies if 
// double-click event is reported to the data source.


using System;
namespace Circus.Wpf.Data {
    /// <summary>Provides an attribute that is applied to the class definition which specifies if double-click event is reported to the data source.</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class DoubleClickVisibleAttribute : Attribute {
        /// <summary>Determine if double-click event is reported.</summary>
        public bool Value { get; private set; }
        private DoubleClickVisibleAttribute() { 
        }
        /// <summary>Constructs a DoubleClickVisibleAttribute with the specified value.</summary>
        public DoubleClickVisibleAttribute(bool value) {
            this.Value = value;
        }
    }
}
