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
// A collapsible menu docked to the side of its container.
//
// Side menu is a tab control that is docked either to the left or right of
// its parent container. The header panel is rotated 90 degrees and items are
// stacked vertically. Items are therefore presented to the side (left or right)
// and are collapsible using the bindable Close command:
//
// <Button Command="{CommandBinding SideMenu, Close}" />
//
// The control is intended to be placed inside a grid that is splitted using
// at least one (or two) grid splitter(s). On template applied, it searches
// for its parent grid and caches the column properties it is placed on using
// an internal ResizeData object. This allows to collapse and restore the width
// of the containing column depending on the IsOpen property.
//
// An example of implementation would be:
//
// <Grid>
//    <Grid.ColumnDefinitions>
//        <ColumnDefinition Width="Auto" />
//        <ColumnDefinition Width="Auto" />
//        <ColumnDefinition Width="*" />
//        <ColumnDefinition Width="Auto" />
//        <ColumnDefinition Width="Auto" />
//    </Grid.ColumnDefinitions>
//    <SideMenu>
//        <SideMenuItem Header="..." >
//            ...
//        </SideMenuItem>
//    </SideMenu>
//    <GridSplitter Grid.Column="1" ResizeBehavior="PreviousAndNext" ResizeDirection="Columns" Width="6" ShowsPreview="True" />
//    <ContentPresenter />
//    <GridSplitter Grid.Column="3" ResizeBehavior="PreviousAndNext" ResizeDirection="Columns" Width="6" ShowsPreview="True" />
//    <SideMenu Dock="Right" Grid.Column="4">
//        <SideMenuItem Header="..." >
//            ...
//        </SideMenuItem>
//    </SideMenu>
// </Grid>
//
// Another option is to use a SideMenuPanel that implements the required
// hierarchy for most use cases (see class comments for details).
//
// Resizing implementation is kept on the object itself rather than on its
// parent container to preserve flexibility of use either by fully defining
// the containing grid or using a packaged container (i.e. SideMenuPanel).
//
// Use IsOpen to specify the initial state. The default value is true.
//
// SizeMode defines the resize behavior applied the first time the control
// is rendered. If not specified, it uses the render pass value and therefore
// sets the width to the computed children widths of the selected item. If a 
// value is provided, the control resizes as follows:
//
// 1) Mode = fixed -> value.
// 2) Mode = auto:
//      a) is opened and value > actual width -> value.
//      b) is opened and value < actual width -> actual width.
//      c) is not opened and value = 0 -> 200.
//
// About c): the item being initially collapsed, there is no way of caching 
// a desired width due to the render pass calculating 0. As a consequence, the
// toggling returns 0 until the control is resized manually.
//
// SizeMode is here to provide a better control on the initial layout as
// children can potentially be very wide and therefore occupy an undesired
// portion of the parent container.


#pragma warning disable IDE0002

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Circus.Wpf.Converters;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a collapsible menu docked to the side of its container.</summary>
    [ClassCommand("Close", ModifierKeys.Shift, Key.Escape)]
    public class SideMenu : TabBase {
        private class ResizeData {
            internal int Column { get; private set; }
            internal ColumnDefinition Definition { get; private set; }
            internal GridSplitter Splitter { get; private set; }
            internal double Width { get; private set; }
            private ResizeData() {
            }
            private ResizeData(int column, ColumnDefinition definition, GridSplitter splitter) {
                this.Column = column;
                this.Definition = definition;
                this.Splitter = splitter;
            }
            internal static ResizeData Create(int column, Grid parent, Dock dock) {
                return ResizeData.IsSideMenu(parent.Children[column]) ? new ResizeData(column, parent.ColumnDefinitions[column], ResizeData.GetSplitter(parent.Children, column, dock)) : null;
            }
            private static GridSplitter GetSplitter(UIElementCollection array, int index, Dock dock) {
                index += dock == Dock.Left ? 1 : -1;
                return index > -1 && array.Count - 1 >= index && array[index] is GridSplitter splitter ? splitter : null;
            }
            private static bool IsSideMenu(UIElement e) {
                return e is SideMenu || (e is ContentPresenter presenter && presenter.Content is SideMenu);
            }
            internal void Update(double width, GridUnitType unit, bool open) {
                this.Width = open ? this.Width : width;
                this.Definition.Width = new GridLength(width, unit);
                if (this.Splitter != null) {
                    this.Splitter.Visibility = BooleanToVisibility.Convert(open);
                }
            }
        }
        private ResizeData data;
        /// <summary>Identifies the dock dependency property.</summary>
        public static readonly DependencyProperty DockProperty;
        /// <summary>Returns the resource key for a templated menu docked to the left.</summary>
        public static readonly ResourceKey LeftTemplateKey;
        /// <summary>Identifies the is open dependency property.</summary>
        public static readonly DependencyProperty IsOpenProperty;
        /// <summary>Returns the resource key for a templated menu docked to the right.</summary>
        public static readonly ResourceKey RightTemplateKey;
        /// <summary>Identifies the size mode dependency property.</summary>
        public static readonly DependencyProperty SizeModeProperty;
        /// <summary>Gets or sets the dock position. Valid values are left or right.</summary>
        [Bindable(true)]
        public Dock Dock { get => (Dock)this.GetValue(SideMenu.DockProperty); set => this.SetValue(SideMenu.DockProperty, value); }
        /// <summary>Gets or sets the visibility of the selected side menu item.</summary>
        [Bindable(true)]
        public bool IsOpen { get => (bool)this.GetValue(SideMenu.IsOpenProperty); set => this.SetValue(SideMenu.IsOpenProperty, value); }
        /// <summary>Gets or sets the width of the control the first time it is rendered.</summary>
        public SizeMode SizeMode { get => (SizeMode)this.GetValue(SideMenu.SizeModeProperty); set => this.SetValue(SideMenu.SizeModeProperty, value); }
        static SideMenu() {
            SideMenu.DockProperty = DependencyProperty.Register("Dock", typeof(Dock), typeof(SideMenu), new FrameworkPropertyMetadata(Dock.Left, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault), SideMenu.IsValidDock);
            SideMenu.LeftTemplateKey = new ComponentResourceKey(typeof(SideMenu), "LeftTemplateKey");
            SideMenu.IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(SideMenu), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(SideMenu.IsOpenChanged)));
            SideMenu.RightTemplateKey = new ComponentResourceKey(typeof(SideMenu), "RightTemplateKey");
            SideMenu.SizeModeProperty = DependencyProperty.Register("SizeMode", typeof(SizeMode), typeof(SideMenu), new FrameworkPropertyMetadata(new SizeMode()));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SideMenu), new FrameworkPropertyMetadata(typeof(SideMenu)));
        }
        /// <summary>Constructs a side menu.</summary>
        public SideMenu() {
        }
        private static void Close(object sender, ExecutedRoutedEventArgs e) {
            ((SideMenu)sender).IsOpen = false;
        }
        protected override DependencyObject GetContainerForItemOverride() {
            return new SideMenuItem();
        }
        private bool GetParent(DependencyObject d, out Grid parent) {
            d = d != null ? VisualTreeHelper.GetParent(d) : null;
            parent = d != null && d is Grid ? (Grid)d : null;
            return parent != null ? true : this.GetParent(d, out parent);
        }
        private bool GetSize(bool opened, out double value, out GridUnitType unit) {
            value = this.SizeMode.Mode == SizeModes.Fixed ? this.SizeMode.Value : !opened && this.SizeMode.Value == 0 ? 200 : this.SizeMode.Value;
            unit = opened && value > 0 && (this.SizeMode.Mode == SizeModes.Fixed || value > this.ActualWidth) ? GridUnitType.Pixel : GridUnitType.Auto;
            return true;
        }
        protected override bool IsItemItsOwnContainerOverride(object item) {
            return item is SideMenuItem;
        }
        private static void IsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((SideMenu)d).IsOpenChanged((bool)e.NewValue);
        }
        private void IsOpenChanged(bool value) {
            if (this.data != null) {
                this.data.Update(value ? this.data.Width : this.ActualWidth, value ? GridUnitType.Pixel : GridUnitType.Auto, this.IsOpen);
            }
        }
        private static bool IsValidDock(object value) {
            Dock dock = (Dock)value;
            return dock == Dock.Left || dock == Dock.Right;
        }
        internal void NotifyItemClicked(SideMenuItem item) {
            this.SelectedItem = item;
            if (!this.IsOpen) {
                this.IsOpen = true;
            }
        }
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            if (this.GetParent(this, out Grid parent)) {
                this.data = ResizeData.Create((int)this.GetValue(Grid.ColumnProperty), parent, this.Dock);
            }
        }
        protected override void OnRenderCompleted(SizeChangedInfo info) {
            if (this.data != null && this.GetSize(this.IsOpen, out double value, out GridUnitType unit)) {
                this.data.Update(value, unit, this.IsOpen);
            }
            base.OnRenderCompleted(info);
        }
    }
}
