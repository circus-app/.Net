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
// An attribute that is applied to the class definition which contains the name
// of the property that is the child node of a hierarchical collection node.


using System;
namespace Circus.Wpf.Data {
    /// <summary>Provides an attribute that is applied to the class definition which contains the name of property that is the child node of a hierarchical collection node.</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class HierarchicalCollectionAttribute : Attribute {
        public string Name { get; private set; }
        private HierarchicalCollectionAttribute() { 
        }
        /// <summary>Constructs a hierarchical collection attribute with the specified property name.</summary>
        public HierarchicalCollectionAttribute(string name) {
            this.Name = name;
        }
    }
}
