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
// A markup extension to support shared data source binding.
//
// This allows to bind the same data source instance to multiple contexts.
// It is useful for cases where several controls must interact with a common
// set of data and avoids declaring static types. As a consequence, a single
// DataSource object can be either local or shared depending on the manner it
// is bound.
//
// A good example is a Search that filters a Treeview. In that case, the 
// Treeview binds to the data collection and Search executes the filtering 
// method on the same instance.
//
// The cache uses the type name as key and therefore, avoids two instances of 
// the same type.
//
// The class uses internally a DataSources collection to cache instances.


#pragma warning disable IDE0002

using System;
using System.Windows.Markup;
using Circus.Wpf.Data;
namespace Circus.Wpf {
    /// <summary>Provides a markup extension to support shared data source binding.</summary>
    [MarkupExtensionReturnType(typeof(DataSource))]
    public sealed class DataBinding : Binding {
        /// <summary>Gets or sets the type that contains the data.</summary>
        public Type Type { get => (Type)base.GetValue(null); set => base.SetValue(value); }
        /// <summary>Constructs a data binding.</summary>
        public DataBinding() { 
        }
        /// <summary>Constructs a data binding with the specified type.</summary>
        public DataBinding(Type type) {
            this.Type = type;
        }
        public override object ProvideValue(IServiceProvider provider) {
            return base.Convert(DataSources.Get(this.Type));
        }
    }
}
