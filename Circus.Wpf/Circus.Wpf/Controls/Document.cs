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
// A headered content control inside a DocumentTray that is draggable and can
// be pinned.
//
// The control implements ISelector interface to provide SelectorView support
// when it is placed on a DocumentTray.
//
// It supports closing by clicking the mouse middle button.
//
// The Icon property is not shown in the header since it is part of the 
// ISelector interface and therefore only applies to the SelectorView.


#pragma warning disable IDE0002

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Circus.Runtime;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a headered content control inside a DocumentTray that is draggable and can be pinned.</summary>
    [ClassCommand("Close", ModifierKeys.Control, Key.F4)]
    [ClassCommand("Float")]
    [ClassCommand("Toggle")]
    public class Document : Primitives.TabItem, ISelector {
        private bool flag;
        private DragInfo info;
        /// <summary>Identifies the resource key for a button style.</summary>
        public static readonly ResourceKey ButtonStyleKey;
        /// <summary>Identifies the closed routed event.</summary>
        public static readonly RoutedEvent ClosedEvent;
        /// <summary>Identifies the icon dependency property.</summary>
        public static readonly DependencyProperty IconProperty;
        /// <summary>Identifies the is toggled dependency property.</summary>
        public static readonly DependencyProperty IsToggledProperty;
        internal Rect Bounds => Screen.GetBounds(this, base.RenderSize, true);
        /// <summary>Gets or sets the icon of the document.</summary>
        public object Icon { get => base.GetValue(Document.IconProperty); set => base.SetValue(Document.IconProperty, value); }
        /// <summary>Determines if the document is toggled.</summary>
        [Bindable(true)]
        public bool IsToggled { get => (bool)base.GetValue(Document.IsToggledProperty); private set => base.SetValue(Document.IsToggledProperty, value); }
        private new DocumentTray Parent => (DocumentTray)ItemsControl.ItemsControlFromItemContainer(this);
        static Document() {
            Document.ButtonStyleKey = new ComponentResourceKey(typeof(Document), "ButtonStyleKey");
            Document.ClosedEvent = EventManager.RegisterRoutedEvent("ClosedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Document));
            Document.HeaderProperty.OverrideMetadata(typeof(Document), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
            Document.IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(Document), new FrameworkPropertyMetadata(Boxes.Null));
            Document.IsToggledProperty = DependencyProperty.Register("IsToggled", typeof(bool), typeof(Document), new FrameworkPropertyMetadata(Boxes.False));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Document), new FrameworkPropertyMetadata(typeof(Document)));
        }
        /// <summary>Constructs a document.</summary>
        public Document() {
            this.flag = false;
        }
        public void Close() {
            this.Parent.NotifyClose(this.DataContext);
            this.OnClosed(new RoutedEventArgs(Document.ClosedEvent, this));
        }
        private static void Close(object sender, ExecutedRoutedEventArgs e) {
            if (Assert.As(sender, out Document document)) {
                document.Close();
            }
        }
        private void EndDrag() {
            if (Allocator.Assign(false, out this.flag)) {
                this.ReleaseMouseCapture();
                this.Parent.NotifyDragCompleted();
            }
        }
        private static void Float(object sender, ExecutedRoutedEventArgs e) {
            if (Assert.As(sender, out Document document)) {
                document.Parent.NotifyFloat(document.DataContext, document.IsToggled);
            }
        }
        private bool IsOutsideSensitivity(Point point) {
            if (this.Bounds.Contains(point)) {
                return false;
            }
            point.Offset(0d - this.info.Origine.X, 0d - this.info.Origine.Y);
            return Math.Abs(point.X) > Screen.MinimumHorizontalDragDistance || Math.Abs(point.Y) > Screen.MinimumVerticalDragDistance;
        }
        /// <summary>Called when the document is closed in a DocumentTray.</summary>
        protected virtual void OnClosed(RoutedEventArgs e) {
            this.RaiseEvent(e);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e) {
            if (Mouse.MiddleButton == MouseButtonState.Pressed) {
                this.Close();
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            if (base.IsMouseCaptured && this.flag) {
                this.EndDrag();
            }
            base.OnMouseLeftButtonUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (Mouse.LeftButton == MouseButtonState.Pressed && !this.flag && this.StartDrag(e.GetPosition(this))) {
                return;
            }
            if (base.IsMouseCaptured && this.flag && Assert.NotNull(PresentationSource.FromDependencyObject(this))  && Allocator.Assign(PointToScreen(e.GetPosition(this)), out Point point)) {
                if (this.info.Started) {
                    this.Parent.NotifyDrag(point);
                }
                else {
                    if (this.IsOutsideSensitivity(point)) {
                        this.info.Started = true;
                        this.Parent.NotifyDragStarted(this.DataContext);
                    }
                }
            }
        }
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
            base.OnMouseRightButtonDown(e);
            if (!this.IsSelected) {
                this.IsSelected = true;
            }
        }
        private bool StartDrag(Point point) {
            if (Allocator.Assign(this.CaptureMouse(), out bool num) && num && Allocator.Assign(new DragInfo(PointToScreen(point)), out this.info)) {
                this.flag = true;
            }
            return num;
        }
        /// <summary>Toggles the document.</summary>
        public void Toggle() {
            this.Toggle(!this.IsToggled);
            this.Parent.NotifyToggled(this.DataContext);
        }
        internal void Toggle(bool value) {
            this.IsToggled = value;
        }
        private static void Toggle(object sender, ExecutedRoutedEventArgs e) {
            if (Assert.As(sender, out Document document)) {
                document.Toggle();
            }
        }
    }
}
