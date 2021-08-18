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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Circus.Text;
namespace Circus.Wpf.Controls {
    internal sealed class DockViewTextBlock : ContentControl {
        private readonly TextBlock block;
		public static readonly DependencyProperty TextProperty;
		[Bindable(true)]
        public string Text { get => (string)this.GetValue(DockViewTextBlock.TextProperty); set => this.SetValue(DockViewTextBlock.TextProperty, value); }
        static DockViewTextBlock() {
            DockViewTextBlock.TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DockViewTextBlock), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockViewTextBlock.OnTextChanged)));
        }
        internal DockViewTextBlock() {
            this.block = new TextBlock();
            base.Content = this.block;
        }
        private bool GetText(out int index, out string value) {
            value = StringInfo.Contains(this.block.Text, "\\", out index) ? this.block.Text.Substring(index, this.block.Text.Length - index) : null;
            return Assert.NotNull(value);
        }
        private bool GetSize(out Size size) {
            size = new Size(double.PositiveInfinity, base.DesiredSize.Height);
            return true;
        }
        private double MeasureText(Size size) {
            this.block.Measure(size);
            return this.block.DesiredSize.Width;
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo size) {
            base.OnRenderSizeChanged(size);
            this.Update();
        }
        private void OnTextChanged(string value) {
            this.block.Text = value;
            this.Update();
        }
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((DockViewTextBlock)d).OnTextChanged((string)e.NewValue);
        }
        private void Trim(Size size, double value) {
            if (this.GetText(out int index, out string text)) {
                int i = 4;
                while (value > base.ActualWidth) {
                    int j = index - i;
                    if (j == -1) {
                        break;
                    }
                    this.block.Text = string.Format("{0}...{1}", this.block.Text.Substring(0, j), text);
                    value = this.MeasureText(size);
                    i++;
                }
            }
        }
        private void Update() {
            if (Assert.NotNull(this.block.Text) && base.ActualWidth > 0 && this.GetSize(out Size size)) {
                double num = this.MeasureText(size);
                if (num > base.ActualWidth) {
                    this.Trim(size, num);
                }
            }
        }
    }
}
