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
// A control that provides an editable content.


namespace Circus.Wpf.Controls {
    /// <summary>Defines a control that provides an editable content.</summary>
    public interface IEditable {
        /// <summary>Presents an IInputElement that allows user input.</summary>
        void Edit();
    }
}
