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
// A document tray drop-down button.
//
// Shows/hides the SelectorView of a DocumentTray (see related classes for 
// details). 
//
// IsOverflow is used for binding to the HasOverflowItems property of a 
// DocumentTrayPanel control. If true, the button content is an overlined 
// arrow.


#pragma warning disable IDE0002

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a document tray drop-down button.</summary>
    public sealed class DocumentTrayDropDownButton : ToggleButton {
        /// <summary>Identifies the is overflow dependency property.</summary>
        public static readonly DependencyProperty IsOverflowProperty;
        /// <summary>Determines if the button content is an overlined arrow.</summary>
        [Bindable(true)]
        public bool IsOverflow { get => (bool)this.GetValue(DocumentTrayDropDownButton.IsOverflowProperty); set => this.SetValue(DocumentTrayDropDownButton.IsOverflowProperty, value); }
        static DocumentTrayDropDownButton() {
            DocumentTrayDropDownButton.IsOverflowProperty = DependencyProperty.Register("IsOverflow", typeof(bool), typeof(DocumentTrayDropDownButton), new FrameworkPropertyMetadata(false));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentTrayDropDownButton), new FrameworkPropertyMetadata(typeof(DocumentTrayDropDownButton)));
        }
        /// <summary>Constructs a DocumentTrayDropDownButton.</summary>
        public DocumentTrayDropDownButton() {
        }
        protected override void OnInitialized(EventArgs e) {
            if (Assert.NotNull(base.ContextMenu)) {
                base.ContextMenu.Placement = PlacementMode.Bottom;
                base.ContextMenu.PlacementTarget = this;
            }
            base.OnInitialized(e);
        }
    }
}
