#pragma warning disable IDE0002

using System.Collections;
using System.Collections.Specialized;
namespace Circus.Collections.Observable {
    public abstract class DependentCollection<T, U> : Cradle<T> where T : IObservable where U : IObservable, new()  {
        public U Items { get => (U)base.GetValue(default); protected set => base.SetValue(value); }
        protected DependentCollection(T source) : base(source) {
            this.Items = new U();
            this.Initialize();
        }
        protected virtual void Initialize() {
        }
        protected virtual void Add(IList array) { 
        }
        protected override void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add: this.Add(e.NewItems); break;
                case NotifyCollectionChangedAction.Remove: this.Remove(e.OldStartingIndex, e.OldItems); break;
                case NotifyCollectionChangedAction.Replace: this.Replace(e.NewStartingIndex, e.OldItems, e.NewItems); break;
                case NotifyCollectionChangedAction.Reset: this.Reset(e.NewItems); break;
            }
            base.OnCollectionChanged(sender, e);
        }
        protected virtual void Remove(int index, IList array) { 
        }
        protected virtual void Replace(int index, IList previous, IList current) { 
        }
        protected virtual void Reset(IList array) {
            this.Items.Clear(true);
            this.Add(array);
        }
    }
}
