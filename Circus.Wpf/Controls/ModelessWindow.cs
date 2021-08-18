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
// A modeless window.
//
// It implements a custom "dialog" behavior using a dispatcher frame that 
// enters a new execution loop to avoid returning when the control is shown
// but without retaining the focus, which allows closing by activating 
// another window.
//
// It is marked as abstract and not styled since the content may 
// considerably vary depending on the context of the popup. Consumer should
// derive from this and implement the desired style.
//
// The class provides a Close routed command that can be used in derived
// types such as:
//
// <Button Command="{CommandBinding ModelessWindow, Close}" ... />
//
// or pressing the escape key. The Result value is false.


#pragma warning disable IDE0002

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a modeless window.</summary>
    [ClassCommand("Close", ModifierKeys.None, Key.Escape)]
    public abstract class ModelessWindow : Window {
        private bool flag;
        private DispatcherFrame frame;
        /// <summary>Returns the window result.</summary>
        public bool Result { get; private set; }
        /// <summary>Constructs a modeless window.</summary>
        protected ModelessWindow() : this(WindowStartupLocation.CenterScreen) {
        }
        /// <summary>Constructs a modeless window with the specified window startup location.</summary>
        protected ModelessWindow(WindowStartupLocation location) {
            ClassCommands.Register(typeof(ModelessWindow));
            base.WindowStartupLocation = location;
            this.flag = true;
        }
        /// <summary>Closes the window using the provided result value.</summary>
        protected virtual void Close(bool result) {
            this.Result = result;
            if (this.Pop()) {
                this.flag = false;
                base.Close();
            }
        }
        private static void Close(object sender, ExecutedRoutedEventArgs e) {
            ((ModelessWindow)sender).Close(false);
        }
        protected override void OnDeactivated(EventArgs e) {
            base.OnDeactivated(e);
            if (this.flag) {
                this.Close(false);
            }
        }
        protected override void OnInitialized(EventArgs e) {
            base.OnInitialized(e);
            if (base.WindowStartupLocation == WindowStartupLocation.CenterScreen && Screen.GetSize(MonitorSize.Work, out Size size)) {
                this.Top = (size.Height - this.Height) / 2.0;
                this.Left = (size.Width - this.Width) / 2.0;
            }
        }
        private bool Pop() {
            bool num = Assert.NotNull(this.frame);
            if (num) {
                this.frame.Continue = false;
                this.frame = null;
                ComponentDispatcher.PopModal();
            }
            return num;
        }
        private bool Push() {
            bool num = Assert.Null(this.frame);
            if (num) {
                ComponentDispatcher.PushModal();
                this.frame = new DispatcherFrame();
            }
            return num;
        }
        /// <summary>Opens a modeless window and returns when it is closed or deactivated.</summary>
        public virtual new bool Show() {
            if (this.Push()) {
                base.Show();
                System.Windows.Threading.Dispatcher.PushFrame(this.frame);
            }
            return this.Result;
        }
    }
}
