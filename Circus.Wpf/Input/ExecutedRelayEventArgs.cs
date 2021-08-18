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
// Event data for an executed relay event.


using System;
using System.Windows.Input;
namespace Circus.Wpf.Input {
    /// <summary>Provides data for an executed relay event.</summary>
    public sealed class ExecutedRelayEventArgs : EventArgs {
        /// <summary>Returns the command that was invoked.</summary>
        public ICommand Command { get; private set; }
        /// <summary>Returns the data parameter of the command.</summary>
        public object Parameter { get; private set; }
        private ExecutedRelayEventArgs() { 
        }
        /// <summary>Constructs an ExecutedRelayEventArgs with the specified ICommand and parameter.</summary>
        public ExecutedRelayEventArgs(ICommand command, object parameter) {
            this.Command = command;
            this.Parameter = parameter;
        }
    }
}
