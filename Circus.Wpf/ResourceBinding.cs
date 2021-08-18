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
// A markup extension to support binding to the resource manager.


#pragma warning disable IDE0002

using System;
using System.Windows.Markup;
namespace Circus.Wpf {
    /// <summary>Provides a markup extension to support binding to the resource manager.</summary>
    [MarkupExtensionReturnType(typeof(object))]
    public sealed class ResourceBinding : Binding {
        /// <summary>Get or sets the resource alias in the ResourceManager.</summary>
        public string Alias { get => (string)base.GetValue(null); set => base.SetValue(value); }
        /// <summary>Get or sets the owner of the cached resource. Default value is null. If specified, uses the ResourceCache instead of the ResourceManager.</summary>
        public object Cache { get => base.GetValue(null); set => base.SetValue(value); }
        /// <summary>Get or sets the name of the resource.</summary>
        public string Name { get => (string)base.GetValue(null); set => base.SetValue(value); }
        /// <summary>Constructs a resource binding.</summary>
        public ResourceBinding() { 
        }
        /// <summary>Constructs a resource binding with the specified alias and name.</summary>
        public ResourceBinding(string alias, string name) {
            this.Alias = alias;
            this.Name = name;
        }
        public ResourceBinding(string alias, string name, object cache) : this(alias, name) {
            this.Cache = cache;
        }
        public override object ProvideValue(IServiceProvider provider) {
            return Assert.NotNull(this.Cache) && ResourceCache.Get(this.Cache, this.Alias, this.Name, out object value) ? base.Convert(value) : ResourceManager.Get(this.Alias, this.Name, out value) ? base.Convert(value) : null;
        }
    }
}
