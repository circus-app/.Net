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


using System;
using System.Windows;
using Circus.Runtime;
namespace Circus.Wpf.Controls {
    internal sealed class DocumentTrayDragInfo : IDisposable {
        private Rect rect;
        internal object Item { get; private set; }
        internal bool Toggled { get; private set; }
        internal DocumentTrayDragInfo(object item, Rect bounds, bool toggled) {
            if (Allocator.Assign(bounds, out this.rect)) {
                this.Item = item;
                this.Toggled = toggled;
            }
        }
        ~DocumentTrayDragInfo() {
            this.Dispose(false);
        }
        private bool Contains(Point point, Rect bounds) {
            return Allocator.Assign(bounds, out this.rect) && this.IsOutsideSensitivity(point);
        }
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) {
            if (disposing && Allocator.Assign(Rect.Empty, out this.rect)) {
                this.Item = null;
            }
        }
        internal bool IsOutsideSensitivity(Point point) {
            return !this.rect.Contains(point);
        }
        internal void Update(Point point, Rect bounds) {
            if (this.Contains(point, bounds)) {
                if (point.X < this.rect.Left) {
                    this.rect.Width += this.rect.Left - point.X;
                    this.rect.X = point.X;
                }
                else {
                    this.rect.Width += point.X - this.rect.Right;
                }
            }
        }
    }
}
