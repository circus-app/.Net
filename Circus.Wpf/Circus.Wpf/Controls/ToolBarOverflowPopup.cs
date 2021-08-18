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
// A pop-up window for toolbars overflow items.
//
// Since its placement is set to the bottom of its parent toolbar overflow
// button, the pop-up is moved each time the toolbar size changes. This is
// especially the case when the user toggles items visibility using the 
// descriptor view.
//
// To prevent this, the pop-up tracks its original position to stay in-place.


using System;
using System.Windows;
using System.Windows.Controls.Primitives;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a pop-up window for toolbars overflow items.</summary>
    public sealed class ToolBarOverflowPopup : Popup {
        /// <summary>Constructs a toolbar overflow pop-up.</summary>
        public ToolBarOverflowPopup() {
        }
        protected override void OnClosed(EventArgs e) {
            this.HorizontalOffset = 0;
            base.OnClosed(e);
        }
        protected override void OnInitialized(EventArgs e) {
            base.OnInitialized(e);
            if (this.TemplatedParent is ToolBar bar) {
                bar.SizeChanged += this.OnParentSizeChanged;
            }
        }
        private void OnParentSizeChanged(object sender, SizeChangedEventArgs e) {
            if (this.IsOpen && e.WidthChanged) {
                this.HorizontalOffset += e.PreviousSize.Width - e.NewSize.Width;
            }
        }
    }
}