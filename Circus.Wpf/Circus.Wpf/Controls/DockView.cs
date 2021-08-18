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

using System;
using System.Windows;
using System.Windows.Input;
using Circus.Runtime;
namespace Circus.Wpf.Controls {
    [ClassCommand("NewWindow")]
    [TemplatePart(Name = "Trays", Type = typeof(ListBox))]
    internal sealed class DockView : ModelessWindow {
        public static readonly ResourceKey ListBoxStyleKey;
        public static readonly ResourceKey TraysListBoxItemStyleKey;
        public static readonly ResourceKey WindowsListBoxItemStyleKey;
        private ListBox Trays { get; set; }
        static DockView() {
            DockView.ListBoxStyleKey = new ComponentResourceKey(typeof(DockView), "ListBoxStyleKey");
            DockView.TraysListBoxItemStyleKey = new ComponentResourceKey(typeof(DockView), "TraysListBoxItemStyleKey");
            DockView.WindowsListBoxItemStyleKey = new ComponentResourceKey(typeof(DockView), "WindowsListBoxItemStyleKey");
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DockView), new FrameworkPropertyMetadata(typeof(DockView)));
        }
        internal DockView(DockViewSource source) {
            this.DataContext = source;
        }
        private static void NewWindow(object sender, ExecutedRoutedEventArgs e) {
            Allocator.Get<DockViewSelectionInfo>(DockViewSelectionInfo.Id).Reset();
            ((DockView)sender).Close(true);
        }
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            TemplateItems.Register(this);
            RoutedEventHandlerManager.AddHandler(this.Trays, ListBox.ItemClickedEvent, new RoutedEventHandler(this.OnItemClicked));
        }
        protected override void OnClosed(EventArgs e) {
            RoutedEventHandlerManager.RemoveHandler(this.Trays);
            ((DockViewSource)this.DataContext).Dispose();
            base.OnClosed(e);
        }
        private void OnItemClicked(object sender, RoutedEventArgs e) {
            base.Close(true);
        }
    }
}
