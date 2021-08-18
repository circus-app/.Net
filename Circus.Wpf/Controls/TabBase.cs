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
// A base container for selectable tab items.
//
// The control provides a render completed event that is raised the first time
// it is rendered right after applying its template. Override this method when
// calculation based on the rendered size of the control is needed.


#pragma warning disable IDE0002

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Circus.Wpf.Controls {
	/// <summary>Provides a base container for selectable tab items.</summary>
	public class TabBase : Selector {
		private bool flag;
		/// <summary>Identifies the selected content dependency property.</summary>
		public static readonly DependencyProperty SelectedContentProperty;
		/// <summary>Returns the content of the currently selected tab item.</summary>
		public object SelectedContent { get => this.GetValue(TabBase.SelectedContentProperty); private set => this.SetValue(TabBase.SelectedContentProperty, value); }
		static TabBase() {
			Control.IsTabStopProperty.OverrideMetadata(typeof(TabBase), new FrameworkPropertyMetadata(false));
			TabBase.SelectedContentProperty = DependencyProperty.Register("SelectedContent", typeof(object), typeof(TabBase), new FrameworkPropertyMetadata(null));
		}
		/// <summary>Constructs a tab.</summary>
		public TabBase() {
			this.flag = false;
		}
		private bool Find(int index, out ContentControl control) {
			for (int i = 0; i < base.Items.Count; i++) {
				index += -1;
				if (index >= base.Items.Count) {
					index = 0;
				}
				else if (index < 0) {
					index = base.Items.Count - 1;
				}
				control = (ContentControl)this.Items[index];
				if (control != null && control.IsEnabled && control.Visibility == Visibility.Visible) {
					return true;
				}
			}
			control = null;
			return false;
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			if (this.SelectedIndex == -1 && this.Items.Count > 0) {
				this.SelectedIndex = 0;
			}
		}
		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			if (e.Action == NotifyCollectionChangedAction.Remove && base.SelectedIndex == -1) {
				int num = e.OldStartingIndex + 1;
				if (this.Find(num > base.Items.Count ? 0 : num, out ContentControl control)) {
					this.SelectedItem = control;
				}
			}
		}
		/// <summary>Raised when the control is rendered after templating.</summary>
		protected virtual void OnRenderCompleted(SizeChangedInfo info) {
			this.flag = true;
		}
		protected override void OnRenderSizeChanged(SizeChangedInfo info) {
			base.OnRenderSizeChanged(info);
			if (!this.flag) {
				this.OnRenderCompleted(info);
			}
		}
		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			this.SelectedContent = base.SelectedItem != null && base.SelectedItem is HeaderedContentControl control ? control.Content : null;
			base.OnSelectionChanged(e);
		}
	}
}
