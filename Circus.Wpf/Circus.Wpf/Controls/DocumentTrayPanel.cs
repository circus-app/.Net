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
// A panel to position and arrange DocumentTray headers.
//
// Pinned items are arranged in sequential position from left to right, 
// breaking content to the next line at the right edge of the panel.
//
// Unpinned items are stacked horizontally either using the same line as 
// pinned items if these are on a single line and the remaining space is 
// enough to display at least one element, or breaking content to a new 
// line. 
//
// Items that do not fit within the panel bounds are pushed to an internal
// overflow array that is updated when the underlying collection changes or
// when the panel is resized.


#pragma warning disable IDE0002

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Circus.Collections;
using Circus.Runtime;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a panel to position and arrange DocumentTray headers.</summary>
    public sealed class DocumentTrayPanel : Panel {
        private class Layout : IDisposable {
            private double offset;
            private Size size;
            internal double Height { get; private set; }
            internal int Rows { get; private set; }
            internal Layout(Size size) {
                if (Allocator.Assign(size, out this.size) && Allocator.Assign(0d, out this.offset)) {
                    this.Height = double.NaN;
                    this.Rows = 1;
                }
            }
            ~Layout() {
                this.Dispose(false);
            }
            internal void Break() {
                if (Allocator.Assign(0d, out this.offset)) {
                    this.Rows++;
                }
            }
            public void Dispose() {
                this.Dispose(true);
            }
            private void Dispose(bool disposing) {
                if (disposing && Allocator.Assign(System.Windows.Size.Empty, out this.size)) {
                    this.Height = double.NaN;
                    this.Rows = 0;
                    GC.SuppressFinalize(this);
                }
            }
            internal void EnsureHeight(double value) {
                if (double.IsNaN(this.Height)) {
                    this.Height = value;
                }
            }
            internal bool IsOverflow(UIElement element) {
                return this.offset + element.DesiredSize.Width > this.size.Width;
            }
            internal void Measure(UIElement element) {
                element.Measure(this.size);
                this.EnsureHeight(element.DesiredSize.Height);
                if (this.IsOverflow(element)) {
                    this.Break();
                }
                this.offset += element.DesiredSize.Width;
            }
            internal Size Size() {
                return new Size(this.size.Width, double.IsNaN(this.Height) ? 0d : this.Height * this.Rows);
            }
        }
        private Set<int> array;
        private bool flag;
        private Layout layout;
        /// <summary>Identifies the has overflow items dependency property.</summary>
        public static readonly DependencyProperty HasOverflowItemsProperty;
        /// <summary>Determines if the panel contains overflow items.</summary>
        [Bindable(true)]
        public bool HasOverflowItems { get => (bool)base.GetValue(DocumentTrayPanel.HasOverflowItemsProperty); private set => base.SetValue(DocumentTrayPanel.HasOverflowItemsProperty, Boxes.Box(value)); }
        static DocumentTrayPanel() {
            DocumentTrayPanel.HasOverflowItemsProperty = DependencyProperty.Register("HasOverflowItems", typeof(bool), typeof(DocumentTrayPanel), new FrameworkPropertyMetadata(Boxes.False));
        }
        /// <summary>Constructs a document tray panel.</summary>
        public DocumentTrayPanel() {
        }
        private int Arrange(Size size, out Point point) {
            int i = 0;
            if (Allocator.Assign(new Point(), out point)) {
                for (; i < base.InternalChildren.Count; i++) {
                    if (this.Find(i, out UIElement element)) {
                        if (Assert.As(element, out Document document) && !document.IsToggled) {
                            break;
                        }
                        if (point.X + element.DesiredSize.Width > size.Width) {
                            point.X = 0d;
                            point.Y += this.layout.Height;
                        }
                        element.Arrange(new Rect(point, new Size(element.DesiredSize.Width > size.Width ? size.Width : element.DesiredSize.Width, element.DesiredSize.Height)));
                        point.X += element.DesiredSize.Width;
                    }
                }
                if (this.layout.Rows > 1) {
                    point = new Point(0d, point.Y + this.layout.Height);
                }
            }
            return i;
        }
        private void Arrange(double width, Point point, int index) {
            if (Allocator.Assign(new Set<int>(base.InternalChildren.Count - index), out this.array)) {
                for (int i = index; i < base.InternalChildren.Count; i++) {
                    if (this.Find(i, out UIElement element)) {
                        Size size = point.X + element.DesiredSize.Width > width ? point.X == 0d ? new Size(width, element.DesiredSize.Height) : new Size(0d, 0d) : element.DesiredSize;
                        if (size.Width == 0d) {
                            this.array.Add(i);
                        }
                        element.Arrange(new Rect(new Point(point.X, point.Y), size));
                        point.X += element.DesiredSize.Width;
                    }
                }
                this.HasOverflowItems = this.array.Count > 0;
            }
        }
        protected override Size ArrangeOverride(Size size) {
            if (Allocator.Assign(this.Arrange(size, out Point point), out int index) && Allocator.Assign(this.layout.Rows > 1, out this.flag)) {
                this.Arrange(size.Width, point, index);
                this.layout.Dispose();
            }
            return size;
        }
        private bool Find(int index, out UIElement element) {
            return Allocator.Assign(base.InternalChildren[index], out element) && element.IsVisible;
        }
        /// <summary>Invalidates the layout if it contains overflow items or if elements are arranged on multiple lines. This is used to force a layout pass when an item is toggled but not moved.</summary>
        public void Invalidate() {
            if (this.flag || this.HasOverflowItems) {
                this.InvalidateMeasure();
            }
        }
        /// <summary>Determines if the item at the specified index is outside the panel bounds.</summary>
        public bool IsOutsideVisibility(int index) {
            return this.HasOverflowItems && this.array.Contains(index);
        }
        private int Measure(Size size, out Layout info) {
            int num = 0;
            if (Allocator.Assign(new Layout(size), out info)) {
                for (; num < base.InternalChildren.Count; num++) {
                    if (this.Find(num, out UIElement element)) {
                        if (Assert.As(element, out Document document) && !document.IsToggled) {
                            break;
                        }
                        info.Measure(element);
                    }
                }
            }
            return num;
        }
        protected override Size MeasureOverride(Size size) {
            if (Allocator.Assign(this.Measure(size, out this.layout), out int num) && Allocator.Assign(true, out bool flag)) {
                for (; num < base.InternalChildren.Count; num++) {
                    if (this.Find(num, out UIElement element)) {
                        element.Measure(size);
                        if (flag) {
                            this.layout.EnsureHeight(element.DesiredSize.Height);
                            if (this.layout.Rows > 1 || this.layout.IsOverflow(element)) {
                                this.layout.Break();
                            }
                            flag = false;
                        }
                    }
                }
            }
            return this.layout.Size();
        }
    }
}
