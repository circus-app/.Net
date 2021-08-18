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
// A base class for a bindable data source.
//
// Since it inherits from Binding, the data source can be naturally bound
// to the DataContext of a control if it is intended to be local.
//
// Use DataBinding instead if it needs to be shared across multiple contexts.
//
// A set of virtual data event handlers that are most commonly used across the 
// package are defined but, since it is a generic component that is intended to 
// be bound to any type of control, it belongs to the consumer to implement their 
// desired behavior.
//
// To keep it simple and avoid multiple casts along the project, it is preferred 
// to have a single DataSource type that covers all use cases instead of  
// different specialized types for control's families (i.e. filterable controls,
// items controls, etc.). As a consequence, not all event handlers are relevant
// in a particular context.


#pragma warning disable CS1998, IDE0002

using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Markup;
using Circus.Runtime;

namespace Circus.Wpf.Data {
    /// <summary>Provides a base class for a bindable data source.</summary>
    [MarkupExtensionReturnType(typeof(DataSource))]
    public abstract class DataSource : Binding, IDisposable {
        private bool disposed;
        /// <summary>Occurs when a drag and drop operation completed.</summary>
        public event EventHandler<DragEventArgs> DragCompleted;
        /// <summary>Occurs when the value of a filter request changed.</summary>
        public event EventHandler<DataEventArgs> FilterValueChanged;
        /// <summary>Occurs when an item is double-clicked.</summary>
        public event EventHandler<DataEventArgs> ItemDoubleClicked;
        /// <summary>Occurs when a registered property of an item changed.</summary>
        public event EventHandler<PropertyChangedEventArgs> ItemPropertyChanged;
        /// <summary>Occurs when an item is removed.</summary>
        public event EventHandler<DataEventArgs> ItemRemoved;
        /// <summary>Occurs when the selected item of a selector control changed.</summary>
        public event EventHandler<DataEventArgs> SelectedItemChanged;
        /// <summary>Determines if item double-click event is reported.</summary>
        public bool DoubleClickVisible { get; private set; }
        /// <summary>Constructs a data source.</summary>
        protected DataSource() {
            if (Allocator.Assign(false, out this.disposed)) {
                this.Initialize();
            }
        }
        ~DataSource() {
            this.Dispose(false);
        }
        /// <summary>Releases all resources used by the DataSource.</summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>Releases all resources used by the DataSource.</summary>
        protected virtual void Dispose(bool disposing) {
            if (this.disposed) {
                return;
            }
            this.disposed = true;
        }
        private void Initialize() {
            this.DoubleClickVisible = Allocator.Assign(this.GetType().GetCustomAttribute<DoubleClickVisibleAttribute>(false), out DoubleClickVisibleAttribute a) && Assert.NotNull(a) && a.Value;
        }
        /// <summary>Invokes the specified data event handler with the provided sender and data event args.</summary>
        public void Invoke(DataEventHandler handler, object sender, DataEventArgs e) {
            handler.Invoke(sender, e);
        }
        /// <summary>Invokes the specified drag event handler with the provided sender and drag event args.</summary>
        public void Invoke(DragEventHandler handler, object sender, DragEventArgs e) {
            handler.Invoke(sender, e);
        }
        /// <summary>Invokes the specified property changed event handler with the provided sender and property changed event args.</summary>
        public void Invoke(PropertyChangedEventHandler handler, object sender, PropertyChangedEventArgs e) {
            handler.Invoke(sender, e);
        }
        /// <summary>Invokes the specified data event handler asynchronously with the provided sender and data event args and returns the resulting Task.</summary>
        public async Task InvokeAsync(DataEventHandlerAsync handler, object sender, DataEventArgs e) {
            await handler.Invoke(sender, e);
        }
        /// <summary>The event handler for a drag completed event.</summary>
        public virtual void OnDragCompleted(object sender, DragEventArgs e) {
            this.DragCompleted?.Invoke(sender, e);
        }
        /// <summary>The event handler for a filter value change.</summary>
        public virtual async Task OnFilterValueChanged(object sender, DataEventArgs e) {
            this.FilterValueChanged?.Invoke(sender, e);
        }
        /// <summary>The event handler for an item double-click event.</summary>
        public virtual void OnItemDoubleClicked(object sender, DataEventArgs e) {
            this.ItemDoubleClicked?.Invoke(sender, e);
        }
        /// <summary>The event handler for a registered item property changed.</summary>
        public virtual void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.ItemPropertyChanged?.Invoke(sender, e);
        }
        /// <summary>The event handler for an item removed event.</summary>
        public virtual void OnItemRemoved(object sender, DataEventArgs e) {
            this.ItemRemoved?.Invoke(sender, e);
        }
        /// <summary>The event handler for a selected item changed.</summary>
        public virtual void OnSelectedItemChanged(object sender, DataEventArgs e) {
            this.SelectedItemChanged?.Invoke(sender, e);
        }
        public override object ProvideValue(IServiceProvider provider) {
            return this;
        }
    }
}
