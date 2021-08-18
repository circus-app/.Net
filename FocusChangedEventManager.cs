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
// A weak event manager to track focus changes on a dependency object 
// children.
//
// The component uses an internal weak map of dependency objects that are 
// tracked and their last keyboard focused element as IInputElement.
//
// Lost focus events are managed by the LostFocusEventManager.
//
// This is used to forward keyboard focus to a specific child of a 
// dependency object when the dependency object itself is activated (see
// Document for an example of implementation).


#pragma warning disable IDE0002

using System.Windows;
using System.Windows.Input;
using Circus.Collections.Conditional;
using Circus.Runtime;
namespace Circus.Wpf {
    /// <summary>Provides a weak event manager to track focus changes on a dependency object children.</summary>
    public sealed class FocusChangedEventManager {
        private readonly WeakTable<DependencyObject, IInputElement> array;
        private static FocusChangedEventManager Current => Allocator.Singleton<FocusChangedEventManager>();
        private FocusChangedEventManager() {
            this.array = new WeakTable<DependencyObject, IInputElement>();
        }
        private void Add(DependencyObject d) {
            if (this.array.Add(d, null)) {
                LostFocusEventManager.AddHandler(d, this.OnLostFocus);
            }
        }
        /// <summary>Adds a focus changed handler to the specified dependency object.</summary>
        public static void AddHandler(DependencyObject d) {
            FocusChangedEventManager.Current.Add(d);
        }
        /// <summary>Outputs the IInputElement associated to the specified dependency object. Returns true if the provided scope contains a handler.</summary>
        public static bool Get(DependencyObject d, out IInputElement element) {
            return FocusChangedEventManager.Current.Find(d, out element);
        }
        private bool Find(DependencyObject d, out IInputElement element) {
            return this.array.Get(d, out element);
        }
        private void OnLostFocus(object sender, RoutedEventArgs e) {
            UIElement element = (UIElement)sender;
            if (element.IsKeyboardFocusWithin) {
                this.array.AddOrUpdate(element, Keyboard.FocusedElement);
            }
        }
        private void Remove(DependencyObject d) {
            if (this.array.Remove(d)) {
                LostFocusEventManager.RemoveHandler(d, this.OnLostFocus);
            }
        }
        /// <summary>Removes the focus changed handler associated to the specified dependency object.</summary>
        public static void RemoveHandler(DependencyObject d) {
            FocusChangedEventManager.Current.Remove(d);
        }
    }
}
