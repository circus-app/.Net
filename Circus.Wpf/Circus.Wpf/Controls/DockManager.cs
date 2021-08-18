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
// A manager of docking operations.
//
// The component caches each document tray available using the register/
// unregister methods when an instance is created or destroyed
// (see DocumenTray for details). 
//
// It uses an internal map which key is the owner since a window can host
// multiple document trays. When a new document document tray is registered
// the component hooks its window owner close event to maintain an updated
// cache without invoking an explicit removal.
//
// The map value is an observable set because it is used for binding in a 
// dock view instance as described below.
// 
// Docking operations are managed using the Request/Commit methods.
//
// Request determines the document tray target for a specific document. If
// the application only contains one window which contains only one
// document tray, the target is a new floating window. Otherwise, it shows
// a dock view window that allows to select the target window and document
// tray for the currently selected document.
//
// Dock view binds to an internal DockViewSource object that contains 
// windows and document trays information. It filters the document tray 
// that requested docking since a document cannot be moved to its current 
// container.
//
// Dock view selection info are stored in a shared instance of 
// DockViewSelectionInfo created on request and used for commitment (see
// Allocator for details on shared instances).
//
// Since DockViewSelectionInfo uses Allocator and to avoid collisions with 
// consumer shared instances, its name is a guid.
//
// A request returns a boolean indicating if a valid dock target has been
// defined since it can be cancelled.
//
// Commit moves the specified document to a new floating window or to the 
// selected document tray.


#pragma warning disable IDE0002

using System;
using System.Windows;
using Circus.Collections;
using Circus.Collections.Observable;
using Circus.Runtime;
using Circus.Wpf.Data;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a manager of docking operations.</summary>
    public sealed class DockManager {
        private readonly Map<Window, ObservableSet<DocumentTray>> array;
        private static DockManager Current => Allocator.Singleton<DockManager>();
        private DockManager() {
            this.array = new Map<Window, ObservableSet<DocumentTray>>();
        }
        private bool Add(DocumentTray source) {
            if (Allocator.Assign(this.array.Get(source.Owner, out ObservableSet<DocumentTray> array), out bool num) && !num) {
                num = this.array.Add(source.Owner, new ObservableSet<DocumentTray>(2, new DocumentTray[] { source }));
                if (num) {
                    source.Owner.Closed += this.OnWindowClosed;
                }
            }
            else if (!array.Contains(source)) {
                num = array.Add(source);
            }
            return num;
        }
        /// <summary>Commits a docking operation by moving the provided document to a new floating window or to the requested document tray.</summary>
        public static void Commit(DocumentTray source, object item, bool toggled) {
            DockManager.Current.Move(source, item, toggled);
        }
        private void Move(DocumentTray source, object item, bool toggled) {
            if ((Allocator.Assign(Allocator.Get<DockViewSelectionInfo>(DockViewSelectionInfo.Id), out DockViewSelectionInfo info) & (Assert.Null(info) || info.Empty)) && FloatingWindow.Create(out FloatingWindow window)) {
                window.Initialize(source, item, toggled);
            }
            else {
                info.Tray.Children.Items.Insert(info.Tray.StackInfo.Next, item);
                info.Tray.Owner.Activate();
            }
            Allocator.Release(DockViewSelectionInfo.Id);
        }
        private void OnWindowClosed(object sender, EventArgs e) {
            this.Remove((Window)sender);
        }
        private bool Query(DocumentTray source) {
            return (this.array.Count == 1 && this.array[source.Owner].Count == 1) || (Assert.As(source.ItemContainerGenerator.ContainerFromItem(source.SelectedItem), out ISelector selector) && this.ShowDialog(source.Owner, selector));
        }
        /// <summary>Registers the provided document tray to the dock manager.</summary>
        public static bool Register(DocumentTray source) {
            return DockManager.Current.Add(source);
        }
        private bool Remove(DocumentTray source) {
            if (Allocator.Assign(this.array.Get(source.Owner, out ObservableSet<DocumentTray> array), out bool num) && num) {
                num = array.Remove(source);
                if (array.Empty) {
                    num = this.Remove(source.Owner);
                }
            }
            return num;
        }
        private bool Remove(Window window) {
            if (Allocator.Assign(this.array.Remove(window), out bool num) && num) {
                window.Closed -= this.OnWindowClosed;
            }
            return num;
        }
        /// <summary>Requests the dock manager for a valid dock target using the provided document tray source and the currently selected document. Returns true if the docking operation can be commited.</summary>
        public static bool Request(DocumentTray source) {
            return DockManager.Current.Query(source);
        }
        private bool ShowDialog(Window owner, ISelector selector) {
            if (Allocator.Assign(Allocator.Make<DockViewSelectionInfo>(DockViewSelectionInfo.Id) && new DockView(new DockViewSource(this.array, owner, selector)).Show(), out bool num) && !num) {
                Allocator.Release(DockViewSelectionInfo.Id);
            }
            return num;
        }
        /// <summary>Unregisters the provided document tray from the dock manager.</summary>
        public static bool Unregister(DocumentTray source) {
            return DockManager.Current.Remove(source);
        }
    }
}
