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
// A lightweight control for displaying an icon and a text that is editable.
//
// Styling is composed of two content presenters. A regular presenter that is
// bound to the content property of the content control and, a named presenter
// that is registered at runtime using the TemplateItems component and bound to
// the text property.
//
// Since icon is added to the content property, caption should be set in the 
// text property.
//
// Editing is achieved by replacing the content of the text presenter with an 
// EditableItemTextBox. On commit, the content is restored to the text property
// (see ITransaction for details about transactions).
//
// This allows to preserve a lightweight control for display and switch to an
// editable textbox only on demand.
//
// The control provides an Edit routed command that switches to the edit mode
// if it is editable and not yet editing.
// 
// When overriden, the GetContainerForEditableItemOverride() allows to specify
// another type as input control (most probably inherited from EditableItemTextBox).
// This is mainly intended for cases where a more sophisticated selection mode is 
// required (see EditableItemTextBox for details).


#pragma warning disable IDE0002

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a lightweight control for displaying an icon and a text that is editable.</summary>
    [ClassCommand("Edit")]
    [TemplatePart(Name = "TextPresenter", Type = typeof(ContentPresenter))]
    public class EditableLabel : ContentControl, IEditable, ITransaction {
        /// <summary>Identifies the icon dependency property.</summary>
        public static readonly DependencyProperty IconProperty;
        /// <summary>Identifies the is editable dependency property.</summary>
        public static readonly DependencyProperty IsEditableProperty;
        /// <summary>Identifies the is editing dependency property.</summary>
        public static readonly DependencyProperty IsEditingProperty;
        /// <summary>Identifies the selection mode dependency property.</summary>
        public static readonly DependencyProperty SelectionModeProperty;
        /// <summary>Identifies the text dependency property.</summary>
        public static readonly DependencyProperty TextProperty;
        /// <summary>Gets or sets the icon of the control.</summary>
        [Bindable(true)]
        public object Icon { get => this.GetValue(EditableLabel.IconProperty); set => this.SetValue(EditableLabel.IconProperty, value); }
        /// <summary>Determines if the control is editable.</summary>
        [Bindable(true)]
        public bool IsEditable { get => (bool)this.GetValue(EditableLabel.IsEditableProperty); set => this.SetValue(EditableLabel.IsEditableProperty, value); }
        /// <summary>Determines if the control is currently editing.</summary>
        [Bindable(true)]
        public bool IsEditing { get => (bool)this.GetValue(EditableLabel.IsEditingProperty); private set => this.SetValue(EditableLabel.IsEditingProperty, value); }
        /// <summary>Gets or sets the selection mode.</summary>
        [Bindable(true)]
        public EditableSelectionMode SelectionMode { get => (EditableSelectionMode)this.GetValue(EditableLabel.SelectionModeProperty); set => this.SetValue(EditableLabel.SelectionModeProperty, value); }
        /// <summary>Gets or sets the text of the control.</summary>
        [Bindable(true)]
        public string Text { get => (string)this.GetValue(EditableLabel.TextProperty); set => this.SetValue(EditableLabel.TextProperty, value); }
        private ContentPresenter TextPresenter { get; set; }
        static EditableLabel() {
            EditableLabel.IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(EditableLabel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(EditableLabel.OnIconChanged)));
            EditableLabel.IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(EditableLabel), new FrameworkPropertyMetadata(true));
            EditableLabel.IsEditingProperty = DependencyProperty.Register("IsEditing", typeof(bool), typeof(EditableLabel), new FrameworkPropertyMetadata(false));
            EditableLabel.SelectionModeProperty = DependencyProperty.Register("SelectionMode", typeof(EditableSelectionMode), typeof(EditableLabel), new FrameworkPropertyMetadata(EditableSelectionMode.Default));
            EditableLabel.TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableLabel), new FrameworkPropertyMetadata(null , FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableLabel), new FrameworkPropertyMetadata(typeof(EditableLabel)));
        }
        /// <summary>Constructs an editable label.</summary>
        public EditableLabel() {
        }
        public void Commit(TransactionResult result, bool focus, object value) {
            if (result == TransactionResult.Commit) {
                this.Text = (string)value;
            }
            if (this.TextPresenter.Content is IDisposable disposable) {
                disposable.Dispose();
            }
            this.OnCommit(focus);
        }
        public void Edit() {
            if (base.IsEnabled && this.IsEditable && !this.IsEditing && this.TextPresenter != null) {
                this.Focusable = true;
                this.TextPresenter.Content = this.GetContainerForEditableItemOverride();
            }
        }
        private static void Edit(object sender, ExecutedRoutedEventArgs e) {
            ((EditableLabel)sender).Edit();
        }
        /// <summary>When overriden creates a new control that will be used as editing text host.</summary>
        public virtual object GetContainerForEditableItemOverride() {
            return new EditableItemTextBox(this, this.SelectionMode, this.Text);
        }
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            TemplateItems.Register(this);
        }
        private void OnCommit(bool focus) {
            this.TextPresenter.Content = this.Text;
            if (focus) {
                this.Focus();
            }
            this.Focusable = false;
        }
        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            d.SetValue(EditableLabel.ContentProperty, ResourceManager.Get(e.NewValue, out object value) ? value : e.NewValue);
        }
    }
}
