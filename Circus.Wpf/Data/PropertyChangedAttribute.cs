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
// An attribute that is applied to the class definition which contains a list of
// properties that are registered to the property changed manager.


using System;
using System.Collections.Generic;
namespace Circus.Wpf.Data {
    /// <summary>Provides an attribute that is applied to the class definition which contains a list of properties that are registered to the property changed manager.</summary>
    [Obsolete("Use Circus.Model.ProppertyChangedAttribute instead")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PropertyChangedAttribute : Attribute {
        /// <summary>Returns an enumeration of property names.</summary>
        public IEnumerable<string> Values { get; private set; }
        private PropertyChangedAttribute() { 
        }
        /// <summary>Constructs a property change attribute with the specified properties names.</summary>
        public PropertyChangedAttribute(params string[] array) {
            this.Values = array;
        }
    }
}
