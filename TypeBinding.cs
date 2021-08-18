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
// A markup extension to support binding to an instance of the specified type.
//
// This is intended for cases when an element is the root of a xaml and for
// which we need an instance of type T bound to a property.
//
// A good example is to bind a menu in the header property of a window.
// Since window is the root of the xaml definition, it is not possible to
// use a StaticResource because xaml does not allow to refer to a resource
// that has not been previously declared.


#pragma warning disable IDE0002

using System;
using System.Windows.Markup;
namespace Circus.Wpf {
    /// <summary>Provides a markup extension to support binding to an instance of the specified type.</summary>
    [MarkupExtensionReturnType(typeof(object))]
    public sealed class TypeBinding : Binding {
        /// <summary>Returns the type of the instance to create.</summary>
        public Type Type { get => (Type)base.GetValue(null); set => base.SetValue(value); }
        /// <summary>Constructs a type binding.</summary>
        public TypeBinding() { 
        }
        /// <summary>Constructs a type binding with the specified type.</summary>
        public TypeBinding(Type type) {
            this.Type = type;
        }
        public override object ProvideValue(IServiceProvider provider) {
            return base.Convert(Activator.CreateInstance(this.Type, true));
        }
    }
}
