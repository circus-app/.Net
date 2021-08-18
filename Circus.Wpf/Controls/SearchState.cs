﻿// Copyright (c) 2019-2020, Circus.
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
// An enumeration of states for a search control.


namespace Circus.Wpf.Controls {
    /// <summary>Provides an enumeration of states for a search control.</summary>
    public enum SearchState : byte {
        /// <summary>The search completed.</summary>
        Completed,
        /// <summary>The control is waiting for a user input.</summary>
        Pending
    }
}
