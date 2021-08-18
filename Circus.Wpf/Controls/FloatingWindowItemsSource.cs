using System.Collections;
using Circus.Collections.Observable;
namespace Circus.Wpf.Controls {
    internal sealed class FloatingWindowItemsSource : DocumentTrayItemsSource {
        internal FloatingWindowItemsSource(IObservable source, DocumentTrayStackInfo info) : base(source, info) {
        }
        protected override void Add(IList array) {
        }
        protected override void Initialize() {
        }
    }
}
