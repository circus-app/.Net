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
// A selection control with a drop-down list.
//
// The control implements IDescriptor to provide DescriptorView
// support when it is placed on a Toolbar.
//
// It provides an optional sort description to allow items sorting
// based on a specific property. Its value can be defined using the
// SortDescriptor extension like this:
//
// <ComboBox Sort="{SortDescriptor Name=SomePropertyName, _
//      Direction=Ascending}"> ...
//
// Since items can either be selectable or execute commands, an internal
// sort is applied, based on the item SortIndex property. Selectable items
// are ordered at the top section of the drop-down and command items at
// the bottom. If SortDescription is not specified, items are presented
// in the order they are declared.
//
// Sort applies to both selectable and executable items.
//
// Executable items are not selectable and therefore do not update
// the selected item property.
//
// The control is not editable.


#pragma warning disable IDE0002

using System.ComponentModel;
using System.Windows;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a selection control with a drop-down list.</summary>
    public class ComboBox : System.Windows.Controls.ComboBox, IDescriptor {
        /// <summary>Identifies the description dependency property.</summary>
        public static readonly DependencyProperty DescriptionProperty;
        /// <summary>Identifies the icon dependency property.</summary>
        public static readonly DependencyProperty IconProperty;
        /// <summary>Identifies the input gesture text dependency property.</summary>
        public static readonly DependencyProperty InputGestureTextProperty;
        /// <summary>Identifies the sort dependency property.</summary>
        public static readonly DependencyProperty SortProperty;
        /// <summary>Gets or sets the user-friendly description of the control.</summary>
        public string Description { get => (string)this.GetValue(ComboBox.DescriptionProperty); set => this.SetValue(ComboBox.DescriptionProperty, value); }
        /// <summary>Gets or sets the icon of the control.</summary>
        public object Icon { get => this.GetValue(ComboBox.IconProperty); set => this.SetValue(ComboBox.IconProperty, value); }
        /// <summary>Gets or sets the shortcut text if access key is specified.</summary>
        public string InputGestureText { get => (string)this.GetValue(ComboBox.InputGestureTextProperty); set => this.SetValue(ComboBox.InputGestureTextProperty, value); }
        /// <summary>Gets or sets the SortDescription used to sort items.</summary>
        public SortDescription Sort { get => (SortDescription)this.GetValue(ComboBox.SortProperty); set => this.SetValue(ComboBox.SortProperty, value); }
        static ComboBox() {
            ComboBox.DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(ComboBox), new FrameworkPropertyMetadata(null));
            ComboBox.IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(ComboBox), new FrameworkPropertyMetadata(null));
            ComboBox.InputGestureTextProperty = DependencyProperty.Register("InputGestureText", typeof(string), typeof(ComboBox), new FrameworkPropertyMetadata(null));
            ComboBox.SortProperty = DependencyProperty.Register("Sort", typeof(SortDescription), typeof(ComboBox), new FrameworkPropertyMetadata(new SortDescription(), new PropertyChangedCallback(ComboBox.OnSortChanged)));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBox), new FrameworkPropertyMetadata(typeof(ComboBox)));
        }
        /// <summary>Constructs a combo box.</summary>
        public ComboBox() {
            this.Items.IsLiveSorting = true;
            this.AddSort("SortIndex");
        }
        private void AddSort(string name) {
            this.AddSort(new SortDescription(name, ListSortDirection.Ascending));
        }
        private void AddSort(SortDescription description) {
            this.Items.SortDescriptions.Add(description);
            this.Items.LiveSortingProperties.Add(description.PropertyName);
        }
        protected override DependencyObject GetContainerForItemOverride() {
            return new ComboBoxItem();
        }
        protected override bool IsItemItsOwnContainerOverride(object item) {
            return Assert.Is<ComboBoxItem>(item);
        }
        private static void OnSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((ComboBox)d).OnSortChanged(e.NewValue);
        }
        private void OnSortChanged(object value) {
            if (this.Items.SortDescriptions.Count > 1) {
                this.Items.SortDescriptions.RemoveAt(1);
            }
            if (Assert.NotNull(value)) {
                this.AddSort((SortDescription)value);
            }
        }
    }
}
