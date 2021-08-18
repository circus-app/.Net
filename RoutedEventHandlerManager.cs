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
// A weak manager of routed events handlers for specified routed events 
// that are registered to the handler collection of elements.
//
// This is a wrapper around UIElement.AddHandler(RoutedEvent, Delegate).
// It provides a centralized container of handlers and allows to register
// multiple handlers in a single statement. Moreover, it allows to remove
// all handlers at once for a specific element which makes code less error
// prone since it is not necessary to provide the list of registered 
// handlers.
//
// The component uses a WeakMap to track elements and their related
// handlers.


#pragma warning disable IDE0002

using System;
using System.Windows;
using System.Collections.Generic;
using Circus.Collections;
using Circus.Collections.Conditional;
using Circus.Runtime;
namespace Circus.Wpf {
    /// <summary>Provides a weak manager of routed events handlers for specified routed events that are registered to the handler collection of elements.</summary>
    public sealed class RoutedEventHandlerManager {
        private readonly WeakMap<UIElement, Map<RoutedEvent, Delegate>> array;
        private static RoutedEventHandlerManager Current => Allocator.Singleton<RoutedEventHandlerManager>();
        private RoutedEventHandlerManager() {
            this.array = new WeakMap<UIElement, Map<RoutedEvent, Delegate>>();
        }
        private Map<RoutedEvent, Delegate> Add(UIElement element) {
            if (!this.array.Get(element, out Map<RoutedEvent, Delegate> map)) {
                map = new Map<RoutedEvent, Delegate>();
                this.array.Add(element, map);
            }
            return map;
        }
        private void Add(UIElement element, params RoutedEventHandlerInfo[] array) {
            Map<RoutedEvent, Delegate> map = this.Add(element);
            foreach (RoutedEventHandlerInfo info in array) {
                this.Add(element, map, info.Event, info.Handler);
            }
        }
        private void Add(UIElement element, RoutedEvent e, Delegate handler) {
            this.Add(element, this.Add(element), e, handler);
        }
        private void Add(UIElement element, Map<RoutedEvent, Delegate> map, RoutedEvent e, Delegate handler) {
            map.AddOrUpdate(e, handler);
            element.AddHandler(e, handler);
        }
        /// <summary>Adds a routed event handler for the provided routed event, adding the handler to the handler collection on the specified element.</summary>
        public static void AddHandler(UIElement element, RoutedEvent e, Delegate handler) {
            RoutedEventHandlerManager.Current.Add(element, e, handler);
        }
        /// <summary>Adds the routed event handlers provided in the RoutedEventHandlerInfo array to the handler collection on the specified element.</summary>
        public static void AddHandler(UIElement element, params RoutedEventHandlerInfo[] array) {
            RoutedEventHandlerManager.Current.Add(element, array);
        }
        private void Remove(UIElement element) {
            if (this.array.Get(element, out Map<RoutedEvent, Delegate> array) && this.array.Remove(element)) {
                foreach (KeyValuePair<RoutedEvent, Delegate> pair in array) {
                    element.RemoveHandler(pair.Key, pair.Value);
                }
            }
        }
        private void Remove(UIElement element, params RoutedEvent[] array) {
            if (this.array.Get(element, out Map<RoutedEvent, Delegate> map)) {
                foreach (RoutedEvent e in array) {
                    if (map.Get(e, out Delegate value) && map.Remove(e)) {
                        element.RemoveHandler(e, value);
                    }
                }
                _ = map.Empty && this.array.Remove(element);
            }
        }
        /// <summary>Removes the event handlers owned by the specified element.</summary>
        public static void RemoveHandler(UIElement element) {
            RoutedEventHandlerManager.Current.Remove(element);
        }
        /// <summary>Removes the event handlers attached to the provided routed event list owned by the specified element.</summary>
        public static void RemoveHandler(UIElement element, params RoutedEvent[] array) {
            RoutedEventHandlerManager.Current.Remove(element, array);
        }
    }
}
