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
// A dependent collection for a document tray items source.
//
// 


#pragma warning disable IDE0002

using System;
using System.Collections;
using System.Collections.Generic;
using Circus.Collections.Observable;
using Circus.Runtime;
namespace Circus.Wpf.Controls {
    public class DocumentTrayItemsSource : DependentCollection<IObservable, Basket<object>> {
        private class Lock : IDisposable {
            private DocumentTrayItemsSource source;
            internal Lock(DocumentTrayItemsSource source) {
                if (Allocator.Assign(source, out this.source)) {
                    this.source.Locked = true;
                }
            }
            ~Lock() {
                this.Dispose(false);
            }
            void IDisposable.Dispose() {
                this.Dispose(true);
            }
            private void Dispose(bool disposing) {
                if (disposing) {
                    this.source.Locked = false;
                    if (Allocator.Assign(null, out this.source)) {
                        GC.SuppressFinalize(this);
                    }
                }
            }
        }
        private readonly DocumentTrayStackInfo info;
        public bool Locked { get; private set; }
        public DocumentTrayItemsSource(IObservable source, DocumentTrayStackInfo info) : base(source) {
            if (Allocator.Assign(info, out this.info)) {
                this.Locked = false;
            }
        }
        protected override void Add(IList array) {
            foreach (object obj in array) {
                base.Items.Insert(this.info.Next, obj);
            }
        }
        protected override void Initialize() {
            if (!base.Source.Empty && Assert.As(base.Source, out IEnumerable<object> array)) {
                base.Items = new Basket<object>(array);
            }
        }
        public void Move(object obj, int index, int position) {
            using (new Lock(this)) {
                base.Items.Remove(index);
                base.Items.Insert(position, obj);
            }
        }
        protected override void Remove(int index, IList array) {

            // Do not use index since item position may have changed 
            // during drag. Moreover source collection could be hash
            // based in which case index is not relevant.
            foreach (object obj in array) {
                if (base.Items.Contains(obj, out int position)) {
                    base.Items.Remove(position);
                }
            }
        }
        protected override void Replace(int index, IList previous, IList current) {
            for (int i = 0; i < previous.Count; i++) {
                if (base.Items.Contains(previous[i], out int position)) {
                    base.Items[position] = current[i];
                }
            }
        }
    }
}
