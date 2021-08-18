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
// Information about a routed event and its delegate.
//
// This is a small object that is mainly intended for registering multiple
// handlers at once to the RoutedEventHandlerManager.


#pragma warning disable CS0660, CS0661

using System;
using System.Windows;
namespace Circus.Wpf {
    /// <summary>Provides information about a routed event and its delegate.</summary>
    public sealed class RoutedEventHandlerInfo {
        /// <summary>Returns the targeted routed event.</summary>
        public RoutedEvent Event { get; private set; }
        /// <summary>Returns the delegate that will handle the routed event.</summary>
        public Delegate Handler { get; private set; }
        /// <summary>Constructs a RoutedEventHandlerInfo with the specified routed event and its corresonding delegate.</summary>
        public RoutedEventHandlerInfo(RoutedEvent routed, Delegate handler) {
            this.Event = routed;
            this.Handler = handler;
        }
        /// <summary>Determines whether specified objects are equivalent.</summary>
        public static bool operator ==(RoutedEventHandlerInfo first, RoutedEventHandlerInfo second) {
            return first.Event.Equals(second.Event) && first.Handler.Equals(second.Handler);
        }
        /// <summary>Determines whether specified objects are not equivalent.</summary>
        public static bool operator !=(RoutedEventHandlerInfo first, RoutedEventHandlerInfo second) {
            return !first.Event.Equals(second.Event) || !first.Handler.Equals(second.Handler);
        }
    }
}
