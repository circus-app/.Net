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
// A textbox for an editable control.
//
// The control takes an ITransaction as parameter in ctors and calls the Commit
// method on key down or lost keyboard focus.
//
// On initialized, it uses the GetSelectionInfoOverride to determine the initial
// selection infos based on the provided EditableSelectionMode.
//
// if EditableSelectionMode is None, the caret is placed at the end of the text.
//
// Selection infos are represented by a Duple<int> that returns the selection
// start and the selection length.
//
// Controls that need more complex selection rules (i.e. file names without
// extensions, file paths segments, etc.) should derive from this and override 
// the GetSelectionInfoOverride() to implement the desired behavior. 


#pragma warning disable IDE0002

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a textbox for an editable control.</summary>
    public class EditableItemTextBox : TextBox, IDisposable {
        private readonly EditableSelectionMode mode;
        private ITransaction transaction;
        /// <summary>Determines if the instance is disposed.</summary>
        public bool IsDisposed { get; private set; }
        private EditableItemTextBox() {
            this.IsDisposed = false;
        }
        /// <summary>Constructs an editable item textbox with the specified ITransaction, the default selection mode and the provided text.</summary>
        public EditableItemTextBox(ITransaction transaction, string text) : this(transaction, EditableSelectionMode.Default, text) { 
        }
        /// <summary>Constructs an editable item textbox with the specified ITransaction, selection mode and text.</summary>
        public EditableItemTextBox(ITransaction transaction, EditableSelectionMode mode, string text) : this() {
            this.transaction = transaction;
            this.mode = mode;
            base.Text = text;
            this.SetSelection();
        }
        ~EditableItemTextBox() {
            this.Dispose(false);
        }
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>Disposes managed resources.</summary>
        protected void Dispose(bool disposing) {
            if (!this.IsDisposed) {
                if (disposing) {
                    this.transaction = null;
                }
                this.IsDisposed = true;
            }
        }
        /// <summary>When overriden creates a new selection infos that will be used for the initial selection.</summary>
        public virtual Duple<int> GetSelectionInfoOverride() {
            int num = base.Text.Length;
            return this.mode == EditableSelectionMode.None ? new Duple<int>(num, 0) : new Duple<int>(0, num);
        }
        protected override Size MeasureOverride(Size constraint) {
            Size size = base.MeasureOverride(constraint);
            size.Width += 5.0;
            return size;
        }
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            if (this.IsDisposed) {
                return;
            }
            if (InputManager.Current.IsInMenuMode && e.NewFocus is ContextMenu menu && menu.PlacementTarget != this) {
                this.transaction.Commit(TransactionResult.Cancel, false, null);
            }
            else {
                this.transaction.Commit(TransactionResult.Commit, false, base.Text);
            }
            base.OnLostKeyboardFocus(e);
        }
        protected override void OnKeyDown(KeyEventArgs e) {
            if (this.IsDisposed) {
                return;
            }
            switch (e.Key) {
                case Key.Return:
                    this.transaction.Commit(TransactionResult.Commit, true, base.Text);
                    e.Handled = true;
                    break;
                case Key.Escape:
                    this.transaction.Commit(TransactionResult.Cancel, true, null);
                    e.Handled = true;
                    break;
            }
            base.OnKeyDown(e);
        }
        protected override void OnRender(DrawingContext context) {
            base.OnRender(context);
            Keyboard.Focus(this);
        }
        private void SetSelection() {
            Duple<int> info = this.GetSelectionInfoOverride();
            base.SelectionStart = info.First;
            base.SelectionLength = info.Second;
        }
    }
}
