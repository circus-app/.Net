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
// A closable headered content control.
//
// Header contains a title, a grip that fills the center panel and a close
// button docked to the right. The close button is disabled if no command
// is specified.
//
// The header is not selectable but it highlights when a child receives
// the focus.
//
// The title part is placed on a content presenter that contains a textblock.
// Since text trimming does not work in auto grid columns, the control
// calculates the desired size of the textblock and adjusts its parent
// presenter width to the rendered size accordingly, using the HeaderWidth
// property.
//
// HeaderSideMargin corresponds to the computed left and right margins
// applied to the header presenter in parent items (i.e. grid, borders, etc.).
// This value is added to the measure text calculation to determine the
// real rendered size of the header. It is template-defined to avoid
// looping through the entire hierarchy at runtime to compute parent
// items thicknesses.
//
// The control is designed to be used in collapsible panels like side menus,
// splitted grids, etc.


#pragma warning disable IDE0002

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Circus.Wpf.Input;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a closable headered content control.</summary>
    public class HeaderedControl : HeaderedContentControl  {
        private readonly TextBlock block;
        private double width;
        /// <summary>Returns the resource key for a button style.</summary>
        public static readonly ResourceKey ButtonStyleKey;
        /// <summary>Identifies the command dependency property.</summary>
        public static readonly DependencyProperty CommandProperty;
        /// <summary>Identifies the command parameter dependency property.</summary>
        public static readonly DependencyProperty CommandParameterProperty;
        /// <summary>Identifies the header side margin dependency property.</summary>
        public static readonly DependencyProperty HeaderSideMarginProperty;
        /// <summary>Identifies the header width dependency property.</summary>
        public static readonly DependencyProperty HeaderWidthProperty;
        /// <summary>Gets or sets the command to execute when the close button is pressed.</summary>
        public ICommand Command { get => (ICommand)this.GetValue(HeaderedControl.CommandProperty); set => this.SetValue(HeaderedControl.CommandProperty, value); }
        /// <summary>Gets or sets the command parameter.</summary>
        public object CommandParameter { get => this.GetValue(HeaderedControl.CommandParameterProperty); set => this.SetValue(HeaderedControl.CommandParameterProperty, value); }
        /// <summary>Gets or sets the computed left and right header margins.</summary>
        public double HeaderSideMargin { get => (double)this.GetValue(HeaderedControl.HeaderSideMarginProperty); set => this.SetValue(HeaderedControl.HeaderSideMarginProperty, value); }
        /// <summary>Returns the actual header width.</summary>
        public double HeaderWidth { get => (double)this.GetValue(HeaderedControl.HeaderWidthProperty); private set => this.SetValue(HeaderedControl.HeaderWidthProperty, value); }
        static HeaderedControl() {
            HeaderedControl.ButtonStyleKey = new ComponentResourceKey(typeof(HeaderedControl), "ButtonStyleKey");
            HeaderedControl.CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(HeaderedControl), new FrameworkPropertyMetadata(null, HeaderedControl.OnCommandChanged));
            HeaderedControl.CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(HeaderedControl), new FrameworkPropertyMetadata(null));
            HeaderedControl.HeaderProperty.AddOwner(typeof(HeaderedControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(HeaderedControl.OnHeaderChanged)));
            HeaderedControl.HeaderSideMarginProperty = DependencyProperty.Register("HeaderSideMargin", typeof(double), typeof(HeaderedControl), new FrameworkPropertyMetadata(0.0));
            HeaderedControl.HeaderWidthProperty = DependencyProperty.Register("HeaderWidth", typeof(double), typeof(HeaderedControl), new FrameworkPropertyMetadata(0.0));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderedControl), new FrameworkPropertyMetadata(typeof(HeaderedControl)));
        }
        /// <summary>Constructs a HeaderedControl.</summary>
        public HeaderedControl() {
            this.block = new TextBlock();
            this.Header = this.block;
        }
        private double MeasureText() {
            this.block.Measure(new Size(double.PositiveInfinity, base.DesiredSize.Height));
            return this.block.DesiredSize.Width;
        }
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            RelayCommand.RegisterContext(d, e.NewValue);
        }
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (Assert.Is(e.NewValue, out string value)) {
                ((HeaderedControl)d).OnHeaderChanged(value);
            }
        }
        private void OnHeaderChanged(string value) {
            this.block.Text = value;
            this.width = this.MeasureText();
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);
            this.HeaderWidth = this.ActualWidth < this.width ? this.ActualWidth - this.HeaderSideMargin : this.width;
        }
    }
}
