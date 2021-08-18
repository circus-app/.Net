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
// A floating window that contains a document tray to host undocked 
// documents.
//
// Since floating window is destroyed when its document tray is empty, at 
// least one document is required when the instance is initialized.
//
// Window title is a concatenation of the application main window title and 
// the currently selected document. Therefore, title is updated on 
// selection change.
//
// Window icon is the icon of the currently selected document. It is 
// provided by a ResourceCache object to avoid creating a new instance of 
// the icon on each selection change and therefore optimize memory usage 
// (see ResourceCache for details).
//
// The window registers application main window routed events that provide
// shortcut keys to ensure accessibility since it is not part of the main 
// window hierarchy.


#pragma warning disable IDE0002

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Circus.Collections.Observable;
using Circus.Runtime;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a floating windows that contains a document tray to host undocked documents.</summary>
    [TemplatePart(Name = "Tray", Type = typeof(DocumentTray))]
    public class FloatingWindow : WindowBase {
        private bool flag;
        /// <summary>Identifies the icon dependency property.</summary>
        public new static readonly DependencyProperty IconProperty;
        /// <summary>Returns the icon of the window.</summary>
        [Bindable(true)]
        public new object Icon { get => this.GetValue(FloatingWindow.IconProperty); private set => this.SetValue(FloatingWindow.IconProperty, value); }
        private static new Window Owner => Application.Current.MainWindow;
        private DocumentTray Tray { get; set; }
        static FloatingWindow() {
            ClassCommands.Register(FloatingWindow.Owner.GetType(), typeof(FloatingWindow));
            FloatingWindow.HeaderProperty.OverrideMetadata(typeof(FloatingWindow), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(FloatingWindow.OnHeaderChanged)));
            FloatingWindow.IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(FloatingWindow), new FrameworkPropertyMetadata(Boxes.Null));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatingWindow), new FrameworkPropertyMetadata(typeof(FloatingWindow)));
        }
        private FloatingWindow() {
            this.flag = false;
        }
        public static bool Create(out FloatingWindow window) {
            bool num = Allocator.Assign(new FloatingWindow(), out window);
            window.Show();
            return num;
        }
        private void Initialize(DocumentTray control) {
            this.Tray.DataContext = control.DataContext;
            this.Tray.ItemContainerStyle = control.ItemContainerStyle;
            this.Tray.Resources = control.Resources;
            this.Initialize(control.Children.Source);
        }
        private void Initialize(IObservable source) {
            this.Tray.Children = new FloatingWindowItemsSource(source, this.Tray.StackInfo);
            this.Tray.SetCurrentValue(ItemsControl.ItemsSourceProperty, this.Tray.Children.Items);
        }
        private void Initialize(object item, bool toggled) {
            this.Tray.Children.Items.Add(item);
            if (toggled && Assert.As(this.Tray.ItemContainerGenerator.ContainerFromItem(item), out Document document)) {
                document.Toggle();
            }
        }
        public void Initialize(DocumentTray control, object item, bool toggled) {
            if (Allocator.Assign(true, out this.flag)) {
                RoutedEventHandlerManager.AddHandler(this.Tray, DocumentTray.SelectionChangedEvent, new SelectionChangedEventHandler(this.OnSelectionChanged));
                this.Initialize(control);
                this.Initialize(item, toggled);
                this.OnActivated();
            }
        }
        private void OnActivated() {
            if (Assert.NotNull(this.Tray) && this.Tray.HasItems && Assert.As(this.Tray.ItemContainerGenerator.ContainerFromItem(this.Tray.SelectedItem), out UIElement element)) {
                element.Focus();
            }
        }
        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);
            if (this.flag) {
                this.OnActivated();
            }
        }
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            TemplateItems.Register(this);
        }
        protected override void OnClosed(EventArgs e) {

            // Ensure remaining items are removed from data source if closing
            // is triggered from the window close button.
            if (this.Tray.HasItems) {
                this.Tray.CloseAll();
            }
            RoutedEventHandlerManager.RemoveHandler(this.Tray);
            ResourceCache.Remove(this);
            base.OnClosed(e);
        }
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (Assert.As(d, out FloatingWindow window)) {
                window.Title = string.Format("{0} - {1}", FloatingWindow.Owner.Title, e.NewValue);
            }
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!this.Tray.HasItems) {
                base.Close();
                return;
            }
            if (Assert.As(this.Tray.ItemContainerGenerator.ContainerFromItem(this.Tray.SelectedItem), out ISelector selector)) {
                this.Update(selector);
            }
        }
        private void Update(ISelector selector) {
            this.Header = selector.Header;
            this.Icon = ResourceCache.Get(this, selector.Icon, out object value) ? value : null;
        }
    }
}
