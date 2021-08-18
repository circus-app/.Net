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
// An item inside a tab.
//
// Header only supports text due to the internal content presenter that 
// only contains a TextBlock as child. The text block has a character
// ellipsis text trimming if text overflows the item width.
//
// The header item is placed in an single row tab panel that expands the
// last visible item to its right bound if the item text is trimmed. This
// is why the item calculates a desired width on header changed, based on
// the rendered text width and, its left and right paddings and borders 
// thicknesses.
//
// It is therefore important to declare padding and border thickness in
// style properties and bind child elements to their values if style is
// overriden.


#pragma warning disable IDE0002

using System.Windows;
using System.Windows.Controls;
using Circus.Runtime;
namespace Circus.Wpf.Controls {
    /// <summary>Provides an item inside a tab.</summary>
    public class TabItem : Primitives.TabItem {
		private readonly TextBlock presenter;
		internal double DesiredWidth { get; private set; }
		static TabItem() {
			TabItem.HeaderProperty.OverrideMetadata(typeof(TabItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(TabItem.OnHeaderChanged)));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItem), new FrameworkPropertyMetadata(typeof(TabItem)));
		}
		/// <summary>Constructs a tab item.</summary>
		public TabItem() {
			if (Allocator.Assign(new TextBlock(), out this.presenter)) {
				base.Content = this.presenter;
			}			
		}
		private double MeasureText() {
			this.presenter.Measure(new Size(double.PositiveInfinity, base.DesiredSize.Height));
			return base.BorderThickness.Left + base.Padding.Left + this.presenter.DesiredSize.Width + base.Padding.Right + base.BorderThickness.Right;
		}
		private void OnHeaderChanged(string value) {
			this.presenter.Text = value;
			this.DesiredWidth = this.MeasureText();
		}
		private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (Assert.As(d, out TabItem item) && Assert.As(e.NewValue, out string value)) {
				item.OnHeaderChanged(value);
			}
		}
	}
}
