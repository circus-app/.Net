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
// A relay command binder object to bind to the event handlers that 
// implement the command.


using System.Windows.Input;
namespace Circus.Wpf.Input {
    /// <summary>Binds a relay command to the event handlers that implement the command.</summary>
    public class RelayCommandBinding {
        /// <summary>Occurs when the associated command executes.</summary>
        public event ExecutedRelayEventHandler Executed;
        /// <summary>Returns the associated command.</summary>
        public ICommand Command { get; private set; }
        private RelayCommandBinding() { 
        }
        /// <summary>Constructs a relay command binding with the specified command and handler.</summary>
        public RelayCommandBinding(ICommand command, ExecutedRelayEventHandler handler) {
            this.Command = command;
            this.Executed += handler;
        }
        /// <summary>Raises the executed event with the specified sender and event args.</summary>
        public void OnExecuted(object sender, ExecutedRelayEventArgs e) {
            this.Executed?.Invoke(sender, e);
        }
    }
}
