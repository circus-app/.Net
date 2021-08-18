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
// A manager of class commands.
//
// The component scans static methods of the owner that matches a class
// command attribute defined at class level. The resulting command is 
// stored in a map of map that contains a list of method names and their 
// corresponding command, each mapped to a type.
//
// Commands are registered according to their delegate signature, which 
// determines if it is a RoutedCommand or a RelayCommand.
//
// Get() method registers the class commands if the provided type is not
// yet registered since it is mainly designed and used in CommandBinding 
// (see class definition for details).
//
// Register(Type owner) method allows to register a type manually. It is 
// intended for cases where types are not templated and therefore do not 
// call CommandBinding (i.e ModelessWindow).
//
// Register(Type source, Type owner) allows to copy routed and relay events
// of the source type to the specified owner type. This is intended to 
// forward events to a type that is not a hierachical member of the source 
// type and for which bubbling does not apply (i.e. FloatingWindow).


#pragma warning disable IDE0002

using System;
using System.Reflection;
using System.Windows.Input;
using Circus.Collections;
using Circus.Runtime;
using Circus.Wpf.Input;
namespace Circus.Wpf {
    /// <summary>Provides a manager of class commands.</summary>
    public sealed class ClassCommands {
        private enum CommandType : byte { 
            Relay = 1,
            Routed = 2,
            Unknown = 0
        }
        private class Entry {
            internal ICommand Command { get; private set; }
            internal Delegate Handler { get; private set; }
            internal CommandType Type { get; private set; }
            internal Entry(CommandType type, ICommand command, Delegate handler) {
                this.Command = command;
                this.Handler = handler;
                this.Type = type;
            }
        }
        private readonly Map<Type, Map<string, Entry>> array;
        private readonly Duple<Type[]> delegates;
        private static ClassCommands Current => Allocator.Singleton<ClassCommands>();
        private ClassCommands() {
            this.array = new Map<Type, Map<string, Entry>>();
            this.delegates = new Duple<Type[]>(new Type[] { typeof(object), typeof(ExecutedRelayEventArgs) }, new Type[] { typeof(object), typeof(ExecutedRoutedEventArgs) });
        }
        private static bool CreateCommand(Type owner, string name, CommandType type, ModifierKeys modifiers, Key key, out ICommand command) {
            return Allocator.Assign(type == CommandType.Routed ? new RoutedCommand(name, owner, ClassCommands.GetGestures(modifiers, key)) : type == CommandType.Relay ? (ICommand)new RelayCommand(name, owner) : null, out command) && Assert.NotNull(command);
        }
        private static T CreateHandler<T>(MethodInfo method) where T : Delegate {
            return (T)Delegate.CreateDelegate(typeof(T), method);
        }
        private bool Copy(Type source, Type owner) {
            if (this.array.Contains(owner)) {
                return false;
            }
            if (Allocator.Assign(this.array.Get(source, out Map<string, Entry> array), out bool num) && num) {
                foreach (Entry entry in array.Values()) {
                    num = (entry.Type == CommandType.Routed && ClassCommands.Copy(owner, (RoutedCommand)entry.Command, (ExecutedRoutedEventHandler)entry.Handler)) || (entry.Type == CommandType.Relay && ClassCommands.Copy(owner, (RelayCommand)entry.Command, (ExecutedRelayEventHandler)entry.Handler));
                }
            }
            return num && this.array.Add(owner, new Map<string, Entry>(array));
        }
        public static bool Copy(Type owner, RelayCommand command, ExecutedRelayEventHandler handler) {
            return RelayCommandManager.Register(new RelayCommandBinding(new RelayCommand(command.Name, owner), handler));
        }
        private static bool Copy(Type owner, RoutedCommand command, ExecutedRoutedEventHandler handler) {
            CommandManager.RegisterClassCommandBinding(owner, new System.Windows.Input.CommandBinding(new RoutedCommand(command.Name, owner, command.InputGestures), handler));
            return true;
        }
        private ICommand Find(Type owner, string name) {
            return !this.array.Get(owner, out Map<string, Entry> array) && this.Insert(owner, name, out ICommand result) ? result : array.Get(name, out Entry entry) ? entry.Command : null;
        }
        /// <summary>Returns the registered ICommand for the specified type that matches the provided name. Returns null if not found.</summary>
        public static ICommand Get(Type owner, string name) {
            return ClassCommands.Current.Find(owner, name);
        }
        private bool GetInfo(Type owner, string name, out MethodInfo method, out CommandType type) {
            if (Allocator.Assign(CommandType.Relay, out type) & !ClassCommands.GetMethod(owner, name, this.delegates.First, out method)) {
                type = ClassCommands.GetMethod(owner, name, this.delegates.Second, out method) ? CommandType.Routed : CommandType.Unknown;
            }
            return type > 0 && Assert.NotNull(method);
        }
        private static InputGestureCollection GetGestures(ModifierKeys modifiers, Key key) {
            return modifiers == ModifierKeys.None && key == Key.None ? null : new InputGestureCollection { new KeyGesture(key, modifiers) };
        }
        private static bool GetMethod(Type owner, string name, Type[] types, out MethodInfo method) {
            return Allocator.Assign(owner.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, types, null), out method) && Assert.NotNull(method);
        }
        private bool Insert(Type owner) {
            return !this.array.Contains(owner) && this.Insert(owner, null, out _);
        }
        private bool Insert(Type owner, string name, Entry entry) {
            return this.array.GetOrAdd(owner, new Map<string, Entry>()).Add(name, entry);
        }
        private bool Insert(Type owner, string name, out ICommand result) {
            if (Allocator.Assign(null, out result)) {
                foreach (ClassCommandAttribute attribute in owner.GetCustomAttributes<ClassCommandAttribute>()) {
                    if (this.GetInfo(owner, attribute.Name, out MethodInfo method, out CommandType type) && ClassCommands.CreateCommand(owner, attribute.Name, type, attribute.Modifiers, attribute.Key, out ICommand command) && ClassCommands.Register(owner, method, command, type, out Entry entry) && this.Insert(owner, attribute.Name, entry)) {
                        if (attribute.Name == name) {
                            result = command;
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>Registers the class commands defined at class level for the specified type.</summary>
        public static void Register(Type owner) {
            ClassCommands.Current.Insert(owner);
        }
        /// <summary>Registers the class commands defined at class level in the source type to the specified owner type.</summary>
        public static void Register(Type source, Type owner) {
            ClassCommands.Current.Copy(source, owner);
        }
        private static bool Register(MethodInfo info, ICommand command, out Delegate handler) {
            return Allocator.Assign(ClassCommands.CreateHandler<ExecutedRelayEventHandler>(info), out handler) && Assert.As(handler, out ExecutedRelayEventHandler value) && RelayCommandManager.Register(new RelayCommandBinding(command, value));
        }
        private static bool Register(Type owner, MethodInfo info, ICommand command, out Delegate handler) {
            if (Allocator.Assign(Allocator.Assign(ClassCommands.CreateHandler<ExecutedRoutedEventHandler>(info), out handler) & Assert.As(handler, out ExecutedRoutedEventHandler value), out bool num) && num) {
                CommandManager.RegisterClassCommandBinding(owner, new System.Windows.Input.CommandBinding(command, value));
            }
            return num;
        }
        private static bool Register(Type owner, MethodInfo info, ICommand command, CommandType type, out Entry entry) {
            return Allocator.Assign(null, out Delegate handler) & ((type == CommandType.Routed && ClassCommands.Register(owner, info, command, out handler)) || (type == CommandType.Relay && ClassCommands.Register(info, command, out handler))) & Allocator.Assign(new Entry(type, command, handler), out entry);
        }
    }
}
