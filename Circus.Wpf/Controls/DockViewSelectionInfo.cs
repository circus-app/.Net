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
namespace Circus.Wpf.Controls {
    internal sealed class DockViewSelectionInfo : IDisposable {
        internal const string Id = "08A42F50EBF54A9B9F08945B0B6A8C30";
        private bool disposed;
        internal bool Empty => Assert.Null(this.Tray) && Assert.Null(this.Window);
        internal DocumentTray Tray { get; set; }
        internal Window Window { get; set; }
        internal DockViewSelectionInfo() {
            this.disposed = false;
        }
        ~DockViewSelectionInfo() {
            this.Dispose(false);
        }
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) {
            if (this.disposed) {
                return;
            }
            if (disposing) {
                if (!this.Empty) {
                    this.Reset();
                }
                this.disposed = true;
            }
        }
        internal void Reset() {
            this.Tray = null;
            this.Window = null;
        }
    }
}
