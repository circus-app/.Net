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
// A base class for an headered content control that implements a 
// dispatcher timer.
//
// Timer is processed at the same priority as rendering.
//
// The control raises the virtual OnCompleted() event handler when the 
// interval has elapsed.


#pragma warning disable IDE0002

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a base class for an headered content control that implements a dispatcher timer.</summary>
    public abstract class Dispatcher : HeaderedContentControl {
        /// <summary>Identifies the interval dependency property.</summary>
        public static readonly DependencyProperty IntervalProperty;
        /// <summary>Identifies the is started dependency property.</summary>
        public static readonly DependencyProperty IsStartedProperty;
        private readonly DispatcherTimer timer;
        /// <summary>Gets or sets the timer interval.</summary>
        public double Interval { get => (double)this.GetValue(Controls.Dispatcher.IntervalProperty); set => this.SetValue(Controls.Dispatcher.IntervalProperty, value); }
        /// <summary>Determines if an interval is defined.</summary>
        public bool IsDispatched => this.Interval > 0;
        /// <summary>Determines if the timer is started.</summary>
        public bool IsStarted { get => (bool)this.GetValue(Controls.Dispatcher.IsStartedProperty); private set => this.SetValue(Controls.Dispatcher.IsStartedProperty, value); }
        static Dispatcher() {
            Controls.Dispatcher.IntervalProperty = DependencyProperty.Register("Interval", typeof(double), typeof(Dispatcher), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(Controls.Dispatcher.OnIntervalChanged)));
            Controls.Dispatcher.IsStartedProperty = DependencyProperty.Register("IsStarted", typeof(bool), typeof(Dispatcher), new FrameworkPropertyMetadata(false));
        }
        /// <summary>Constructs a dispatcher.</summary>
        public Dispatcher() {
            this.IsStarted = false;
            this.timer = new DispatcherTimer(DispatcherPriority.Render);
            this.timer.Tick += this.OnCompleted;
        }
        /// <summary>Raised when the interval has elapsed.</summary>
        protected virtual void OnCompleted(object sender, EventArgs e) {
            this.Stop();
        }
        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((Dispatcher)d).OnIntervalChanged((double)e.NewValue);
        }
        private void OnIntervalChanged(double value) {
            if (!this.IsStarted) {
                this.timer.Interval = TimeSpan.FromMilliseconds(value);
            }
        }
        /// <summary>Starts the dispatcher timer. Returns false if already started or is not dispatched.</summary>
        public bool Start() {
            if (!this.IsStarted && this.IsDispatched) {
                this.timer.Start();
                this.IsStarted = true;
            }
            return this.IsStarted;
        }
        /// <summary>Stops the dispatcher timer. Returns true if succeded.</summary>
        public bool Stop() {
            if (this.IsStarted) {
                this.timer.Stop();
                this.IsStarted = false;
            }
            return !this.IsStarted;
        }
        /// <summary>Restarts the dispatcher timer. Returns false if not dispatched.</summary>
        public bool Restart() {
            return this.Stop() && this.Start();
        }
    }
}
