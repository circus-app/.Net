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
// An attribute that is applied to the class definition which contains the 
// name of the ICommand handler to invoke and optionally the KeyGesture 
// associated to the command.


using System;
using System.Windows.Input;
namespace Circus.Wpf {
    /// <summary>Provides an attribute that is applied to the class definition which contains the name of the ICommand handler to invoke and optionally the KeyGesture associated to the command.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ClassCommandAttribute : Attribute {
        /// <summary>Returns the ICommand shortcut key.</summary>
        public Key Key { get; private set; }
        /// <summary>Returns the ICommand modifier key.</summary>
        public ModifierKeys Modifiers { get; private set; }
        /// <summary>Returns the ICommand name.</summary>
        public string Name { get; private set; }
        private ClassCommandAttribute() {
        }
        /// <summary>Constructs a class command attribute with the specified ICommand name.</summary>
        public ClassCommandAttribute(string name) : this(name, ModifierKeys.None, Key.None) {
        }
        /// <summary>Constructs a class command attribute with the specified ICommand name, modifiers keys and shortcut key.</summary>
        public ClassCommandAttribute(string name, ModifierKeys modifiers, Key key) {
            this.Key = key;
            this.Modifiers = modifiers;
            this.Name = name;
        }
    }
}
