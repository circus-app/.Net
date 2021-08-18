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
// A control that presents hierachical data in a tree structure and provides
// filtering.
//
// Basically the same implementation as TreeView except that filtering uses
// a PivotCollection.
//
// This is intended for cases when the filtered result is provided by an external
// process like a webservice or database query that entierly replaces the content
// of the source collection.


#pragma warning disable IDE0002

using System.Collections;
using System.Collections.Specialized;
using Circus.Wpf.Data;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a control that presents hierachical data in a tree structure and supports filtering.</summary>
    public class PivotTreeView : TreeViewBase {
        private enum Flags {
            Filter,
            Reset,
            Restore
        }
        private Flags flag;
        /// <summary>Constructs a PivotTreeView.</summary>
        public PivotTreeView() : base() {
        }
        private static bool Assert(IEnumerable source, out IPivotCollection array) {
            array = source != null && source is IPivotCollection ? (IPivotCollection)source : null;
            return array != null;
        }
        public override void OnItemAdded(TreeViewItem item) {
            if (this.flag == Flags.Restore) {
                base.Source.Restore(item);
            }
            else if (this.flag == Flags.Reset) {
                base.OnItemAdded(item);
            }
        }
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
            base.OnItemsChanged(e);
            if (this.flag == Flags.Filter) {
                base.State = ExpandableState.Expanded;
            }
            else if(this.flag == Flags.Restore) {
                base.Restore();
            }
        }
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
            base.OnItemsSourceChanged(oldValue, newValue);
            this.UnhookCollection(oldValue);
            int num = base.Items.Count;
            if (PivotTreeView.Assert(newValue, out IPivotCollection array)) {
                num = array.Count;
                array.FilterCompleted += this.OnItemsSourceFilterCompleted;
            }
            base.Source = new TreeViewSource(num);
            this.flag = Flags.Reset;
        }
        private void OnItemsSourceFilterCompleted(object sender, FilterCompletedEventArgs e) {
            this.flag = e.IsFiltered ? Flags.Filter : Flags.Restore;
            if (e.IsFiltered) {
                base.Filter(e.IsEmpty);
            }
        }
        private void UnhookCollection(IEnumerable source) {
            if (PivotTreeView.Assert(source, out IPivotCollection array)) {
                array.FilterCompleted -= this.OnItemsSourceFilterCompleted;
            }
        }
    }
}
