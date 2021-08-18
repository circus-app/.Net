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


#pragma warning disable IDE0002

using System.Windows;
using System.Windows.Controls;
namespace Circus.Wpf.Controls {
	internal sealed class TabPanel : Panel {
        internal TabPanel() {
        }
        protected override Size ArrangeOverride(Size size) {
			double num = 0d;
			foreach (UIElement e in base.InternalChildren) {
				e.Arrange(new Rect(num, 0d, e.DesiredSize.Width, e.DesiredSize.Height));
				num += e.DesiredSize.Width;
			}
			return size;
		}
		private bool Find(out TabItem item) {
			int num = base.InternalChildren.Count - 1;
			for (int i = num; i > -1; i--) {
				item = (TabItem)base.InternalChildren[i];
				if (item.Visibility == Visibility.Visible) {
					return true;
				}
			}
			item = null;
			return false;
		}
        protected override Size MeasureOverride(Size size) {
			double height = 0d;
			double width = 0d;
			foreach (UIElement e in base.InternalChildren) {
				e.Measure(new Size(size.Width - width, size.Height - height));
				if (e.DesiredSize.Height > height) {
					height = e.DesiredSize.Height;
				}
				width += e.DesiredSize.Width;
			}
			return new Size(width, height);
		}
		protected override void OnRenderSizeChanged(SizeChangedInfo info) {
			base.OnRenderSizeChanged(info);
			if (this.Find(out TabItem item) && this.Translate(item, out Point point)) {
				item.Arrange(new Rect(point.X, point.Y, item.DesiredWidth > item.ActualWidth ? this.ActualWidth - point.X : item.DesiredWidth, item.ActualHeight));
			}
		}
		private bool Translate(UIElement e, out Point point) {
			point = e.TranslatePoint(new Point(0, 0), this);
			return true;
		}
	}
}
