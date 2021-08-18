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
// A headered toolbar overflow toggle button.
//
// The header property is bound to its parent toolbar header. This allows to 
// specify a custom tooltip that is the concatenation of the header and the 
// default tooltip.
//
// The default tooltip is "ToolBar Options".


#pragma warning disable IDE0002

using System.ComponentModel;
using System.Windows;
namespace Circus.Wpf.Controls {
	/// <summary>Provides a headered toolbar overflow toggle button.</summary>
	public sealed class ToolBarOverflowButton : ToggleButton {
		/// <summary>Identifies the header dependency property.</summary>
		public static readonly DependencyProperty HeaderProperty;
		/// <summary>Binds to the parent toolbar header if specified.</summary>
		[Bindable(true)]
		public string Header { set => this.SetValue(ToolBarOverflowButton.HeaderProperty, value); }
		static ToolBarOverflowButton() {
			ToolBarOverflowButton.HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ToolBarOverflowButton), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ToolBarOverflowButton.OnHeaderChanged)));
			ToolBarOverflowButton.ToolTipProperty.OverrideMetadata(typeof(ToolBarOverflowButton), new FrameworkPropertyMetadata("Toolbar Options"));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBarOverflowButton), new FrameworkPropertyMetadata(typeof(ToolBarOverflowButton)));
		}
		/// <summary>Constructs a toolbar overflow button.</summary>
		public ToolBarOverflowButton() {
        }
		private static object GetToolTip(object header, object tooltip) {
			return header != null ? string.Format("{0} {1}", header, tooltip) : tooltip;
		}
		private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			d.SetValue(ToolBarOverflowButton.ToolTipProperty, ToolBarOverflowButton.GetToolTip(e.NewValue, ToolBarOverflowButton.ToolTipProperty.GetMetadata(typeof(ToolBarOverflowButton)).DefaultValue));
		}
	}
}
