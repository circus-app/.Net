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

using System.Collections;
using System.ComponentModel;
using System.Windows;
using Circus.Collections;
using Circus.Collections.Observable;
using Circus.Runtime;
using Circus.Wpf.Data;
namespace Circus.Wpf.Controls {
    internal sealed class DockViewSource : DataSource {
        private Map<Window, ObservableSet<DocumentTray>> array;
        private bool disposed;
        private Window owner;
        [Bindable(true)]
        public int Count { get => (int)this.GetValue(0); private set => this.SetValue(value); }
        [Bindable(true)]
        public object Icon { get => this.GetValue(null); private set => this.SetValue(value); }
        [Bindable(true)]
        public object Header { get => this.GetValue(null); private set => this.SetValue(value); }
        [Bindable(true)]
        public string Path { get => (string)base.GetValue(null); private set => base.SetValue(value); }
        [Bindable(true)]
        public IEnumerable Trays { get => (IEnumerable)this.GetValue(null); private set => this.SetValue(value); }
        [Bindable(true)]
        public IEnumerable Windows {
            get {
                this.Count = 0;
                foreach (Window window in this.array.Keys()) {
                    if (window != this.owner) {
                        this.Count++;
                        yield return window;
                    }
                }
            }
        }
        internal DockViewSource(Map<Window, ObservableSet<DocumentTray>> array, Window owner, ISelector selector) {
            if (Allocator.Assign(array, out this.array) && Allocator.Assign(false, out this.disposed) && Allocator.Assign(owner, out this.owner)) {
                this.Initialize(selector);
            }
        }
        protected override void Dispose(bool disposing) {
            if (this.disposed) {
                return;
            }
            if (disposing && Allocator.Assign(null, out this.array) && Allocator.Assign(null, out this.owner)) {
                this.disposed = true;
            }
            base.Dispose(disposing);
        }
        private void Initialize(ISelector selector) {
            this.Header = selector.Header;
            this.Icon = ResourceCache.Get(typeof(DockViewSource), selector.Icon, out object value) ? value : null;
        }
        public override void OnSelectedItemChanged(object sender, DataEventArgs e) {
            DockViewSelectionInfo info = Allocator.Get<DockViewSelectionInfo>(DockViewSelectionInfo.Id);
            _ = ((Assert.Is(e.Value, out Window window) && this.Update(ref info, window)) || (Assert.Is(e.Value, out DocumentTray tray) && this.Update(ref info, tray))) && this.Update(info.Window.Title, info.Tray);
        }
        private bool Update(ref DockViewSelectionInfo info, DocumentTray tray) {
            info.Tray = tray;
            return true;
        }
        private bool Update(ref DockViewSelectionInfo info, Window window) {
            this.Trays = this.array[window];
            info.Window = window;
            return true;
        }
        private bool Update(string title, DocumentTray tray) {
            this.Path = string.Format("{0}\\{1}", title, Assert.NotNull(tray) ? tray.Header : null);
            return true;
        }
    }
}
