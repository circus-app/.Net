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
// An indicator of the progress of an operation.
//
// A basic implementation of a progress bar that only supports horizontal
// orientation.


#pragma warning disable IDE0002

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
namespace Circus.Wpf.Controls {
	/// <summary>Provides an indicator of the progress of an operation.</summary>
	[TemplatePart(Name = "Animator", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "Indicator", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "Track", Type = typeof(FrameworkElement))]
	public class Progress : RangeBase {
		private class Animation : ThicknessAnimationUsingKeyFrames {
			private Animation() { 
			}
			internal Animation(Sizes sizes, Durations durations) {
				base.BeginTime = durations.Second;
				base.Duration = new Duration(durations.First + TimeSpan.FromSeconds(0.25));
				base.RepeatBehavior = RepeatBehavior.Forever;
				this.Initialize(sizes, durations.First);
			}
			private void Initialize(Sizes sizes, TimeSpan duration) {
				base.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(sizes.Second, 0.0, 0.0, 0.0), TimeSpan.FromSeconds(0.0)));
				base.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(sizes.First, 0.0, 0.0, 0.0), duration));
			}
		}
		private class Double {
			private Double() { 
			}
			internal static bool AreClose(double value1, double value2) {
				if (value1 == value2) {
					return true;
				}
				double num = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.2204460492503131E-16;
				double num2 = value1 - value2;
				return 0.0 - num < num2 ? num > num2 : false;
			}
			internal static bool GreaterThan(double value1, double value2) {
				return value1 > value2 ? !Double.AreClose(value1, value2) : false;
			}
			internal static bool LessThan(double value1, double value2) {
				return value1 < value2 ? !Double.AreClose(value1, value2) : false;
			}
		}
		private class Durations : Duple<TimeSpan> {
			internal Durations(TimeSpan first, TimeSpan second) : base(first, second) {
			}
		}
		private class Sizes : Duple<double> {
			internal Sizes(double first, double second) : base(first, second) {
			}
		}
		public static readonly DependencyProperty IsIndeterminateProperty;
		private FrameworkElement Animator { get; set; }
		private FrameworkElement Indicator { get; set; }
		/// <summary>Determines if the progress shows the actual value or a continuous feedback.</summary>
		public bool IsIndeterminate { get => (bool)this.GetValue(Progress.IsIndeterminateProperty); set => this.SetValue(Progress.IsIndeterminateProperty, value); }
		private FrameworkElement Track { get; set; }
		static Progress() {
			Progress.FocusableProperty.OverrideMetadata(typeof(Progress), new FrameworkPropertyMetadata(false));
			Progress.IsIndeterminateProperty = DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(Progress), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(Progress.OnIsIndeterminateChanged)));
			Progress.MaximumProperty.OverrideMetadata(typeof(Progress), new FrameworkPropertyMetadata(100.0));
			Progress.VisibilityProperty.OverrideMetadata(typeof(Progress), new FrameworkPropertyMetadata(Visibility.Visible, new PropertyChangedCallback(Progress.OnIsVisibleChanged)));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Progress), new FrameworkPropertyMetadata(typeof(Progress)));
		}
		/// <summary>Constructs a progress.</summary>
		public Progress() {
		}
		private Durations GetDuration(Sizes sizes) {
			double num = this.ActualWidth < 300 ? 200 : 300;
			TimeSpan value = (!Double.GreaterThan(this.Animator.Margin.Left, sizes.Second) || !Double.LessThan(this.Animator.Margin.Left, sizes.First - 1.0)) ? TimeSpan.Zero : TimeSpan.FromSeconds(-1.0 * (this.Animator.Margin.Left - sizes.Second) / num);
			return new Durations(TimeSpan.FromSeconds((int)(sizes.First - sizes.Second) / num), value);
		}
		private Sizes GetSize() {
			return new Sizes(this.Indicator.Width + this.Animator.Width, -1.0 * this.Animator.Width);
		}
		public override void OnApplyTemplate() {
			TemplateItems.Register(this);
		}
		private static void OnIsIndeterminateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Progress)d).UpdateIndicator();
		}
		private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Progress)d).UpdateAnimation();
		}
		protected override void OnMaximumChanged(double old, double value) {
			base.OnMaximumChanged(old, value);
			this.UpdateIndicator();
		}
		protected override void OnMinimumChanged(double old, double value) {
			base.OnMinimumChanged(old, value);
			this.UpdateIndicator();
		}
		protected override void OnRenderSizeChanged(SizeChangedInfo info) {
			base.OnRenderSizeChanged(info);
			this.UpdateIndicator();
		}
		protected override void OnValueChanged(double old, double value) {
			base.OnValueChanged(old, value);
			this.UpdateIndicator();
		}
		private void UpdateAnimation() {
			if (this.Animator != null) {
				Animation animation = null;
				if (base.IsVisible) {
					Sizes sizes = this.GetSize();
					animation = new Animation(sizes, this.GetDuration(sizes));
				}
				this.Animator.BeginAnimation(FrameworkElement.MarginProperty, animation);
			}
		}
		private void UpdateIndicator() {
			if (this.Indicator != null && this.Track != null) {
				this.Indicator.Width = ((this.IsIndeterminate || base.Maximum <= base.Minimum) ? 1.0 : ((base.Value - base.Minimum) / (base.Maximum - base.Minimum))) * this.Track.ActualWidth;
				this.UpdateAnimation();
			}
		}
	}
}
