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
// A markup extension that defines a sort description for collection controls.
//
// This provides a bindable object that returns a sort description based
// on the specified direction and property name.
//
// It is a more convenient way of declaring sort description by wrapping all 
// attributes in a single object directly instanciated from xaml. Since 
// ProvideValue returns a SortDescription, the target control only needs to 
// implement the corresponding dependency property instead of declaring each 
// properties and having to implement coercion between them.
//
// See ComboBox for an example of implementation.
//
// The sort descriptor can be declared without parameters and will therefore
// use its default values:
//
// <Control Sort="{SortDescriptor}"> ...
//
// Default values are Ascending for the sort direction and Content for
// the property name.


#pragma warning disable IDE0002

using System;
using System.ComponentModel;
using System.Windows.Markup;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a markup extension that defines a sort description for collection controls.</summary>
    [MarkupExtensionReturnType(typeof(SortDescription))]
    public sealed class SortDescriptor : Binding {
        /// <summary>Gets or sets the sort direction.</summary>
        public ListSortDirection Direction { get => (ListSortDirection)this.GetValue(ListSortDirection.Ascending); set => this.SetValue(value); }
        /// <summary>Gets or sets the property name used for sorting.</summary>
        public string Name { get => (string)this.GetValue("Content"); set => this.SetValue(value); }
        /// <summary>Constructs a sort descriptor with default values.</summary>
        public SortDescriptor() { 
        }
        /// <summary>Constructs a sort descriptor with the specified property name and the default sort direction.</summary>
        public SortDescriptor(string name) : this(name, ListSortDirection.Ascending) {
        }
        /// <summary>Constructs a sort descriptor with the specified property name and sort direction.</summary>
        public SortDescriptor(string name, ListSortDirection direction) {
            this.Name = name;
            this.Direction = direction;
        }
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return base.Convert(new SortDescription(this.Name, this.Direction));
        }
    }
}
