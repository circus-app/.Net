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
// An item inside a combo box.
//
// The item provides a command and a command parameter that allows command 
// execution on item click.
//
// Since command bound items are sorted at the bottom of their containing
// combo box, a read-only sort index is provided to help parent sorting.
// 0 is a regular item, 1 is a command bound item.


#pragma warning disable IDE0002

using System.Windows;
using System.Windows.Input;
using Circus.Wpf.Input;

namespace Circus.Wpf.Controls {
    /// <summary>Provides an item inside a combo box.</summary>
    public class ComboBoxItem : System.Windows.Controls.ComboBoxItem {
        /// <summary>Identifies the command dependency property.</summary>
        public static readonly DependencyProperty CommandProperty;
        /// <summary>Identifies the command parameter dependency property.</summary>
        public static readonly DependencyProperty CommandParameterProperty;
        /// <summary>Identifies the sort index dependency property.</summary>
        public static readonly DependencyProperty SortIndexProperty;
        /// <summary>Gets or sets the command to execute when the item is pressed.</summary>
        public ICommand Command { get => (ICommand)this.GetValue(ComboBoxItem.CommandProperty); set => this.SetValue(ComboBoxItem.CommandProperty, value); }
        /// <summary>Gets or sets the command parameter.</summary>
        public object CommandParameter { get => this.GetValue(ComboBoxItem.CommandParameterProperty); set => this.SetValue(ComboBoxItem.CommandParameterProperty, value); }
        /// <summary>Returns the sort index based on the command property.</summary>
        public int SortIndex { get => (int)this.GetValue(ComboBoxItem.SortIndexProperty); private set => this.SetValue(ComboBoxItem.SortIndexProperty, value); }
        static ComboBoxItem() {
            ComboBoxItem.SortIndexProperty = DependencyProperty.Register("SortIndex", typeof(int), typeof(ComboBoxItem), new FrameworkPropertyMetadata(0));
            ComboBoxItem.CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ComboBoxItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ComboBoxItem.OnCommandChanged)));
            ComboBoxItem.CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ComboBoxItem), new FrameworkPropertyMetadata(null));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBoxItem), new FrameworkPropertyMetadata(typeof(ComboBoxItem)));
        }
        /// <summary>Constructs a combo box item.</summary>
        public ComboBoxItem() {
            this.SortIndex = 0;
        }
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((ComboBoxItem)d).SortIndex = Assert.NotNull(e.NewValue) ? 1 : 0;
            RelayCommand.RegisterContext(d, e.NewValue);
        }
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e) {
            if (Assert.NotNull(this.Command)) {
                this.Command.Execute(this.CommandParameter);
            }
            base.OnPreviewMouseLeftButtonUp(e);
        }
    }
}
