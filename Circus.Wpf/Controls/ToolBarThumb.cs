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
// A draggable grip for a toolbar.


#pragma warning disable IDE0002

using System.Windows;
using System.Windows.Controls.Primitives;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a draggable grip for a toolbar.</summary>
    public sealed class ToolBarThumb : Thumb {
        static ToolBarThumb() {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBarThumb), new FrameworkPropertyMetadata(typeof(ToolBarThumb)));
        }
        public ToolBarThumb() {
        }
    }
}
