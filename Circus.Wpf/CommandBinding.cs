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
// A markup extension to bind to a class command.
//
// Type is the type that owns the command. Name is the name of the command:
//
// <WindowButton Command="{CommandBinding WindowBase, Minimize}" />...
//
// The class uses an early binding mecanism that registers the class commands
// even if the instance of the specified type is not yet created by the xaml
// engine. This allows to not depend on the hierarchy to bind commands. It is
// especially the case for RelayCommands that do not target a visual element.


#pragma warning disable IDE0002

using System;
using System.Windows.Input;
using System.Windows.Markup;
namespace Circus.Wpf {
    /// <summary>Provides a markup extension to bind to a class command.</summary>
    [MarkupExtensionReturnType(typeof(ICommand))]
    public sealed class CommandBinding : Binding {
        /// <summary>Returns the name of the command.</summary>
        public string Name { get => (string)base.GetValue(null); set => base.SetValue(value); }
        /// <summary>Returns the type that owns the command.</summary>
        public Type Type { get => (Type)base.GetValue(null); set => base.SetValue(value); }
        /// <summary>Constructs a command binding.</summary>
        public CommandBinding() { 
        }
        /// <summary>Constructs a command binding with the specified target type and command name.</summary>
        public CommandBinding(Type type, string name) {
            this.Type = type;
            this.Name = name;
        }
        public override object ProvideValue(IServiceProvider provider) {
            return base.Convert(ClassCommands.Get(this.Type, this.Name));
        }
    }
}
