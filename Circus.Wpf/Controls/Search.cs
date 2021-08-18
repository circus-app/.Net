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
// A search control.
//
// The control provides a textbox, a watermark, a toggle button to execute/clear
// the content and optionally a popup that contains search options.
//
// Since it derives from Dispatcher, an interval can be specified to trigger
// the search once it is elapsed. This allows to automatically execute Update()
// method as long as the content is not whitespaces and it has not changed within 
// the interval.
//
// The default interval is 0, meaning that the dispatcher is disabled.
//
// The control can also execute the filtering method by clicking on the search
// button or by pressing the enter key (only if the textbox contains focus).
// Both are still available even if an interval is specified.
//
// Because it is mostly intended to filter a data source owned by another type,
// the control uses its DataContext to invoke the filtering method when ready.
// This is achieved by listening to DataContext changes and casting the result
// to a DataSource type (if applicable). When search is triggered, it invokes
// asynchronously the OnFilterValueChanged delegate providing its value in the
// DataEventArgs. A null value is valid as it allows to remove the filter on
// the data.
//
// While filtering, it shows a progress that is hidden when the asynchronous 
// Task completed.
//
// The control is specifically designed to work with a DataSource object bound 
// to its DataContext. 
//
// Options provides a presenter to hold advanced search options (i.e. case
// sensitivity, search scope, etc.) or any kind of control most probably
// bound to some data source properties. If not specified, the right most 
// arrow button is hidden. 


#pragma warning disable IDE0002

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Circus.Wpf.Data;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a search control.</summary>
    [ClassCommand("Clear")]
    [ClassCommand("Update")]
    public class Search : Dispatcher {
        private class Monitor {
            private readonly IEqualityComparer<string> comparer;
            internal bool Changed { get; private set; }
            internal bool IsEmpty => this.Text == null;
            internal string Text { get; private set; }
            internal Monitor() {
                this.comparer = new Text.StringComparer();
            }
            private void Compare(string value) {
                bool num = value == null;
                bool num2 = this.Text == null;
                this.Changed = (!num && num2) || (num && !num2) || (!(num && num2) && !this.comparer.Equals(value, this.Text));
                this.Text = value;
            }
            internal void Reset() {
                this.Changed = false;
            }
            internal bool Update(string value) {
                this.Compare(!string.IsNullOrWhiteSpace(value) ? value.Trim() : null);
                return this.Changed;
            }
        }
        private readonly Monitor monitor;
        /// <summary>Returns the resource key for a button style.</summary>
        public static readonly ResourceKey ButtonStyleKey;
        /// <summary>Returns the resource key for a clear geometry.</summary>
        public static readonly ResourceKey ClearGeometryKey;
        /// <summary>Returns the resource key for a search geometry.</summary>
        public static readonly ResourceKey SearchGeometryKey;
        /// <summary>Identifies the is awaiting dependency property.</summary>
        public static readonly DependencyProperty IsAwaitingProperty;
        /// <summary>Identifies the is drop-down open dependency property.</summary>
        public static readonly DependencyProperty IsDropDownOpenProperty;
        /// <summary>Identifies the options dependency property.</summary>
        public static readonly DependencyProperty OptionsProperty;
        /// <summary>Identifies the state dependency property.</summary>
        public static readonly DependencyProperty StateProperty;
        /// <summary>Identifies the text dependency property.</summary>
        public static readonly DependencyProperty TextProperty;
        /// <summary>Identifies the watermark dependency property.</summary>
        public static readonly DependencyProperty WatermarkProperty;
        private DataSource DataSource { get; set; }
        /// <summary>Returns true if the control is currently awaiting a response from the data source.</summary>
        public bool IsAwaiting { get => (bool)this.GetValue(Search.IsAwaitingProperty); private set => this.SetValue(Search.IsAwaitingProperty, value); }
        /// <summary>Returns true if the drop-down part is opened.</summary>
        public bool IsDropDownOpen { get => (bool)this.GetValue(Search.IsDropDownOpenProperty); set => this.SetValue(Search.IsDropDownOpenProperty, value); }
        /// <summary>Gets or sets the control that is presented in the options popup.</summary>
        public object Options { get => this.GetValue(Search.OptionsProperty); set => this.SetValue(Search.OptionsProperty, value); }
        /// <summary>Returns the state of the control.</summary>
        public SearchState State { get => (SearchState)this.GetValue(Search.StateProperty); private set => this.SetValue(Search.StateProperty, value); }
        /// <summary>Returns the actual content.</summary>
        public string Text { get => (string)this.GetValue(Search.TextProperty); internal set => this.SetValue(Search.TextProperty, value); }
        /// <summary>Gets or sets the watermark.</summary>
        public string Watermark { get => (string)this.GetValue(Search.WatermarkProperty); set => this.SetValue(Search.WatermarkProperty, value); }
        static Search() {
            Search.ButtonStyleKey = new ComponentResourceKey(typeof(Search), "ButtonStyleKey");
            Search.ClearGeometryKey = new ComponentResourceKey(typeof(Search), "ClearGeometryKey");
            Search.SearchGeometryKey = new ComponentResourceKey(typeof(Search), "SearchGeometryKey");
            Search.DataContextProperty.OverrideMetadata(typeof(Search), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(Search.OnDataContextChanged)));
            Search.IsAwaitingProperty = DependencyProperty.Register("IsAwaiting", typeof(bool), typeof(Search), new FrameworkPropertyMetadata(false));
            Search.IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(Search), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(Search.OnIsDropDownOpenChanged)));
            Search.OptionsProperty = DependencyProperty.Register("Options", typeof(object), typeof(Search), new FrameworkPropertyMetadata(null));
            Search.StateProperty = DependencyProperty.Register("State", typeof(SearchState), typeof(Search), new FrameworkPropertyMetadata(SearchState.Pending));
            Search.TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Search), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(Search.OnTextChanged)));
            Search.WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(Search), new FrameworkPropertyMetadata(null));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Search), new FrameworkPropertyMetadata(typeof(Search)));
        }
        /// <summary>Constructs a search.</summary>
        public Search() {
            this.monitor = new Monitor();
        }
        private void BeginUpdate() {
            if (this.IsDropDownOpen) {
                this.Close();
            }
            if (!this.monitor.IsEmpty) {
                this.State = SearchState.Completed;
            }
            this.IsAwaiting = true;
        }
        private static void Clear(object sender, ExecutedRoutedEventArgs e) {
            ((Search)sender).Text = null;
        }
        private void Close() {
            this.SetValue(Search.IsDropDownOpenProperty, false);
        }
        private void EndUpdate() {
            this.monitor.Reset();
            this.IsAwaiting = false;
        }
        protected override void OnCompleted(object sender, EventArgs e) {
            base.OnCompleted(sender, e);
            this.Update();
        }
        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((Search)d).DataSource = Assert.Is(e.NewValue, out DataSource data) ? data : null;
        }
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((Search)d).OnIsDropDownOpenChanged((bool)e.NewValue);
        }
        private void OnIsDropDownOpenChanged(bool opened) {
            if (opened) {
                Mouse.Capture(this, CaptureMode.SubTree);
            }
            else {
                if (this.IsKeyboardFocusWithin) {
                    this.Focus();
                }
                if (Mouse.Captured == this) {
                    Mouse.Capture(null);
                }
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            if (!this.IsDropDownOpen && !this.IsKeyboardFocusWithin) {
                this.Focus();
            }
            e.Handled = true;
            if (Mouse.Captured == this && e.OriginalSource == this) {
                this.Close();
            }
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            if (e.Key == Key.Enter && (!this.IsStarted || this.Stop())) {
                this.Update();
            }
            base.OnPreviewKeyDown(e);
        }
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((Search)d).OnTextChanged();
        }
        private void OnTextChanged() {
            if (this.monitor.Update(this.Text)) {
                if (this.State == SearchState.Completed) {
                    this.State = SearchState.Pending;
                }
                base.Restart();
            }
            else {
                base.Stop();
            }
        }
        private void OnUpdate() {
            this.Focus();
            if (this.monitor.Changed) {
                this.Update();
            }
        }
        private static void Update(object sender, ExecutedRoutedEventArgs e) {
            ((Search)sender).OnUpdate();
        }
        private async void Update() {
            if (this.DataSource != null) {
                this.BeginUpdate();
                await this.DataSource.InvokeAsync(this.DataSource.OnFilterValueChanged, this, new DataEventArgs(this.monitor.Text));
                this.EndUpdate();
            }
        }
    }
}
