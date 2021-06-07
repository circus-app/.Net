#pragma warning disable IDE0002

using System;
using System.Collections.Specialized;
using Circus.Model;
using Circus.Runtime;
namespace Circus.Collections.Observable {
    public abstract class Cradle<T> : ObservableObject, IDisposable, INotifyCollectionChanged where T : IObservable {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public T Source { get => (T)base.GetValue(null); private set => base.SetValue(value); }
        protected Cradle(T source) {
            CollectionChangedEventManager.AddHandler(source, this.OnCollectionChanged);
            this.Source = source;
        }
        ~Cradle() {
            this.Dispose(false);
        }
        public void Dispose() {
            this.Dispose(true);
        }
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                CollectionChangedEventManager.RemoveHandler(this.Source, this.OnCollectionChanged);
                GC.SuppressFinalize(this);
            }
        }
        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            this.CollectionChanged?.Invoke(sender, e);
        }
    }
}
