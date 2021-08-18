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
// filtering of its local content.
//
// There are mainly two strategies for filtering treeviews that are bound to a 
// data source. 
// 
// The first one is to use a HierarchicalCollection object that filters the data 
// present in the collection, the second is to replace the content of the data 
// source with a webservice/database query that matches the predicate (use 
// PivotTreeView instead for this scenario). Treeview uses the first option.
//
// On items source changed, the control subscribes to the FilterCompleted event
// if the source is an IHierarchicalCollection and executes the Filter() /
// Restore() methods depending on event args.
//
// Filtering uses a TreeViewLocalSource object that caches the items that are
// excluded to easily restore their visibility when the filter is invalidated or
// changed.
//
// TreeViewLocalSource maintains two caches. On filtered, a first cache is 
// populated with the excluded items. If the filter is invalidated, the cache is
// cleared and the items are restored to their default visibility.
//
// If a new filter is applied while being filtered, another cache is created
// containing the excluded items of the new enumeration. Items of the preceeding
// cache are restored if they are not present in the new cache since they do not
// match the new predicate. Once restored, the old cache is destroyed.
//
// This double-cache startegy avoids restoring the entire treeview before applying
// a new filter and therefore increases performance since toggling items visibility
// is a synchronous operation.
//
// Note that for large data sets (> ~200 items) and to preserve responsiveness it 
// is recommended to use a PivotTreeView.


#pragma warning disable IDE0002

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using Circus.Wpf.Converters;
using Circus.Wpf.Data;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a control that presents hierachical data in a tree structure and implements filtering of its local content.</summary>
    public class TreeView : TreeViewBase {
        private new TreeViewLocalSource Source { get => (TreeViewLocalSource)base.Source; }
        /// <summary>Constructs a tree view.</summary>
        public TreeView() : base() {
        }
        private static bool Assert(IEnumerable source, out IHierarchicalCollection array) {
            array = source != null && source is IHierarchicalCollection ? (IHierarchicalCollection)source : null;
            return array != null;
        }
        /// <summary></summary>
        public virtual void Filter(int count, bool empty) {
            base.Filter(empty);
            base.State = ExpandableState.Expanded;
            Visibility visibility = TreeView.GetVisibility(false);
            this.Source.Reserve(count);
            foreach (object o in this.ItemsSource) {
                if (this.Source.Push(o, out TreeViewItem item)) {
                    item.SetValue(UIElement.VisibilityProperty, visibility);
                }
            }
            this.Restore(false);
        }
        private static Visibility GetVisibility(bool value) {
            return BooleanToVisibility.Convert(value);
        }
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
            base.OnItemsSourceChanged(oldValue, newValue);
            this.UnhookCollection(oldValue);
            int num = base.Items.Count;
            if (TreeView.Assert(this.ItemsSource, out IHierarchicalCollection array)) {
                num = array.Count;
                array.FilterCompleted += this.OnItemsSourceFilterCompleted;
            }
            base.Source = new TreeViewLocalSource(num);
        }
        private void OnItemsSourceFilterCompleted(object sender, HierarchicalFilterCompletedEventArgs e) {
            if (e.IsFiltered) {
                this.Filter(e.Count, e.IsEmpty);
                return;
            }
            this.Restore();
        }
        public override void Restore() {
            this.Restore(true);
            foreach (TreeViewItem item in this.Source.Collapsed()) {
                item.IsExpanded = false;
            }
            base.Restore();
        }
        private void Restore(bool clear) {
            if (this.Source.Pop(clear, out IEnumerable<TreeViewItem> array)) {
                Visibility visibility = TreeView.GetVisibility(true);
                foreach (TreeViewItem item in array) {
                    item.SetValue(UIElement.VisibilityProperty, visibility);
                }
            }
        }
        private void UnhookCollection(IEnumerable source) {
            if (TreeView.Assert(source, out IHierarchicalCollection array)) {
                array.FilterCompleted -= this.OnItemsSourceFilterCompleted;
            }
        }
    }
}
