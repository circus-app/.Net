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
// A chrome window.
//
// Header is added to the non-client area which requires 
// WindowChrome.IsHitTestVisibleInChrome = true if user interaction is needed.
//
// The header presenter fills the remaining space of the window (except window 
// buttons). This is to allow positionning controls either to the left or to 
// the right of the window.
//
// Nevertheless, this could result in an undraggable window if the area is 
// filled with a control that has a background. In that case, make sure that 
// the header control background is set to {x:Null}.
//
// Controls that are mostly intended to be located in this area (i.e WindowMenu) 
// already implement null backgrounds.
//
// OnMouseLeftButtonUp override activates the window if it is yet not active.
// This is to ensure that the keyboard focus is restored to the window since
// many controls (i.e. TextBox, RichTextBox, etc.) do not activate their parent
// window when they're clicked.


#pragma warning disable IDE0002

using System;
using System.Windows;
using System.Windows.Input;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a chrome window.</summary>
    [ClassCommand("Close")]
    [ClassCommand("Maximize")]
    [ClassCommand("Minimize")]
    public class WindowBase : Window {
        /// <summary>Identifies the header property.</summary>
        public static readonly DependencyProperty HeaderProperty;
        /// <summary>Identifies the is closable property.</summary>
        public static readonly DependencyProperty IsClosableProperty;
        /// <summary>Identifies the is main window property.</summary>
        public static readonly DependencyProperty IsMainWindowProperty;
        /// <summary>Identifies the is minimizable property.</summary>
        public static readonly DependencyProperty IsMinimizableProperty;
        /// <summary>Gets or sets the header object.</summary>
        public object Header { get => this.GetValue(WindowBase.HeaderProperty); set => this.SetValue(WindowBase.HeaderProperty, value); }
        /// <summary>Determines if the window has a close button.</summary>
        public bool IsClosable { get => (bool)this.GetValue(WindowBase.IsClosableProperty); set => this.SetValue(WindowBase.IsClosableProperty, value); }
        /// <summary>Determines if the window is the main window of the application.</summary>
        public bool IsMainWindow { get => (bool)this.GetValue(WindowBase.IsMainWindowProperty); set => this.SetValue(WindowBase.IsMainWindowProperty, value); }
        /// <summary>Determines if the window has a minimize button.</summary>
        public bool IsMinimizable { get => (bool)this.GetValue(WindowBase.IsMinimizableProperty); set => this.SetValue(WindowBase.IsMinimizableProperty, value); }
        static WindowBase() {
            WindowBase.HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(WindowBase), new FrameworkPropertyMetadata(null));
            WindowBase.IsClosableProperty = DependencyProperty.Register("IsClosable", typeof(bool), typeof(WindowBase), new FrameworkPropertyMetadata(true));
            WindowBase.IsMainWindowProperty = DependencyProperty.Register("IsMainWindow", typeof(bool), typeof(WindowBase), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(WindowBase.OnIsMainWindowChanged)));
            WindowBase.IsMinimizableProperty = DependencyProperty.Register("IsMinimizable", typeof(bool), typeof(WindowBase), new FrameworkPropertyMetadata(true));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowBase), new FrameworkPropertyMetadata(typeof(WindowBase)));
        }
        /// <summary>Constructs a window.</summary>
        public WindowBase() { 
        }
        private static void Close(object sender, ExecutedRoutedEventArgs e) {
            SystemCommands.CloseWindow((Window)sender);
        }
        private static void Maximize(object sender, ExecutedRoutedEventArgs e) {
            Window window = (Window)sender;
            if (window.WindowState == WindowState.Maximized) {
                SystemCommands.RestoreWindow(window);
                return;
            }
            SystemCommands.MaximizeWindow(window);
        }
        private static void Minimize(object sender, ExecutedRoutedEventArgs e) {
            SystemCommands.MinimizeWindow((Window)sender);
        }
        private static void OnIsMainWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ((bool)e.NewValue) {
                Application.Current.MainWindow = ((Window)d);
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonUp(e);
            if (!this.IsActive) {
                this.Activate();
            }
        }
    }
}
