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
// A command related utility that register relay command binding objects.
//
// Command bindings are mapped to their ICommand in an internal map.


#pragma warning disable IDE0002

using System;
using System.Windows.Input;
using Circus.Collections;
namespace Circus.Wpf.Input {
    /// <summary>Provides a command related utility that register relay command binding objects.</summary>
    public class RelayCommandManager {
        [ThreadStatic]
        private static readonly Map<ICommand, RelayCommandBinding> Map;
        static RelayCommandManager() {
            RelayCommandManager.Map = new Map<ICommand, RelayCommandBinding>();
        }
        private RelayCommandManager() { 
        }
        internal static bool Get(ICommand command, out RelayCommandBinding binding) {
            return RelayCommandManager.Map.Get(command, out binding);
        }
        /// <summary>Registers the provided relay command binding. Returns true if registration succeded.</summary>
        public static bool Register(RelayCommandBinding binding) {
            return RelayCommandManager.Map.Add(binding.Command, binding);
        }
    }
}
