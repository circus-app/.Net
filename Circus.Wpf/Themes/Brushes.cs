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
using System.Windows.Media;
using Circus.Wpf.Images;

namespace Circus.Wpf.Themes {
    public sealed class Brushes {
        private static readonly ResourceDictionary Resources;
        public static object ArrowMouseOver => Brushes.Resources["ArrowMouseOver"];
        public static object ButtonArrowChecked => Brushes.Resources["ButtonArrowChecked"];
        public static object ButtonBackgroundChecked => Brushes.Resources["ButtonBackgroundChecked"];
        public static object ControlBackground => Brushes.Resources["ControlBackground"];
        public static object ControlBackgroundPressed => Brushes.Resources["ControlBackgroundPressed"];
        public static object ControlBackgroundMouseOver => Brushes.Resources["ControlBackgroundMouseOver"];
        public static object ControlBorder => Brushes.Resources["ControlBorder"];
        public static object ControlBorderChecked => Brushes.Resources["ControlBorderChecked"];
        public static object ControlBorderMouseOver => Brushes.Resources["ControlBorderMouseOver"];
        public static object ControlForeground => Brushes.Resources["ControlForeground"];
        public static object ControlForegroundDisabled => Brushes.Resources["ControlForegroundDisabled"];
        public static object ControlForegroundPressed => Brushes.Resources["ControlForegroundPressed"];
        public static object DocumentBackground => Brushes.Resources["DocumentBackground"];
        public static object DocumentBackgroundMouseOver => Brushes.Resources["DocumentBackgroundMouseOver"];
        public static object DocumentBackgroundSelected => Brushes.Resources["DocumentBackgroundSelected"];
        public static object DocumentButtonBackgroundMouseOver => Brushes.Resources["DocumentButtonBackgroundMouseOver"];
        public static object DocumentButtonBackgroundUnselectedMouseOver => Brushes.Resources["DocumentButtonBackgroundUnselectedMouseOver"];
        public static object DocumentButtonForegroundHighlighted => Brushes.Resources["DocumentButtonForegroundHighlighted"];
        public static object DocumentForeground => Brushes.Resources["DocumentForeground"];
        public static object DocumentForegroundFocused => Brushes.Resources["DocumentForegroundFocused"];
        public static object DocumentTrayBorder => Brushes.Resources["DocumentTrayBorder"];
        public static object DocumentTrayBorderFocused => Brushes.Resources["DocumentTrayBorderFocused"];
        public static object GeometryBoxDisabled => Brushes.Resources["GeometryBoxDisabled"];
        public static object Grip => Brushes.Resources["Grip"];
        public static object GripFocused => Brushes.Resources["GripFocused"];
        public static object HeaderedControlButtonBackgroundMouseOver => Brushes.Resources["HeaderedControlButtonBackgroundMouseOver"];
        public static object HeaderedControlButtonBackgroundFocusedMouseOver => Brushes.Resources["HeaderedControlButtonBackgroundFocusedMouseOver"];
        public static object HeaderedControlButtonForegroundMouseOver => Brushes.Resources["HeaderedControlButtonForegroundMouseOver"];

        public static object ListBoxItemBackgroundSelected => Brushes.Resources["ListBoxItemBackgroundSelected"];
        public static object ListBoxItemBackgroundSelectedUnfocused => Brushes.Resources["ListBoxItemBackgroundSelectedUnfocused"];
        public static object ListBoxItemForegroundSelected => Brushes.Resources["ListBoxItemForegroundSelected"];

        public static object MenuItemGlyph => Brushes.Resources["MenuItemGlyph"];
        public static object MenuItemGlyphBackground => Brushes.Resources["MenuItemGlyphBackground"];
        public static object ProgressForeground => Brushes.Resources["ProgressForeground"];
        public static object ScrollBarThumb => Brushes.Resources["ScrollBarThumb"];
        public static object ScrollBarThumbDisabled => Brushes.Resources["ScrollBarThumbDisabled"];
        public static object ScrollBarThumbMouseOver => Brushes.Resources["ScrollBarThumbMouseOver"];
        public static object ScrollBarThumbPressed => Brushes.Resources["ScrollBarThumbPressed"];
        public static object ScrollViewerArrow => Brushes.Resources["ScrollViewerArrow"];
        public static object ScrollViewerArrowDisabled => Brushes.Resources["ScrollViewerArrowDisabled"];
        public static object ScrollViewerArrowMouseOver => Brushes.Resources["ScrollViewerArrowMouseOver"];
        public static object ScrollViewerArrowPressed => Brushes.Resources["ScrollViewerArrowPressed"];
        public static object SearchClearButtonForeground => Brushes.Resources["SearchClearButtonForeground"];
        public static object SelectorViewItemButtonMouseOver => Brushes.Resources["SelectorViewItemButtonMouseOver"];
        public static object Separator => Brushes.Resources["Separator"];
        public static object SeparatorLight => Brushes.Resources["SeparatorLight"];
        public static object TabItemSelectedForeground => Brushes.Resources["TabItemSelectedForeground"];
        public static object TextBoxBackground => Brushes.Resources["TextBoxBackground"];
        public static object TextBoxBorder => Brushes.Resources["TextBoxBorder"];
        public static object TextBoxSelectionBrush => Brushes.Resources["TextBoxSelectionBrush"];
        public static object TextBoxSelectionTextBrush => Brushes.Resources["TextBoxSelectionTextBrush"];
        public static object TreeViewItemBackgroundDragOver => Brushes.Resources["TreeViewItemBackgroundDragOver"];
        public static object TreeViewItemBackgroundSelected => Brushes.Resources["TreeViewItemBackgroundSelected"];
        public static object TreeViewItemBackgroundSelectedUnfocused => Brushes.Resources["TreeViewItemBackgroundSelectedUnfocused"];
        public static object TreeViewItemExpanderMouseOver => Brushes.Resources["TreeViewItemExpanderMouseOver"];
        public static object TreeViewItemForeground => Brushes.Resources["TreeViewItemForeground"];
        public static object TreeViewItemForegroundSelected => Brushes.Resources["TreeViewItemForegroundSelected"];
        public static object Watermark => Brushes.Resources["Watermark"];
        public static object Window => Brushes.Resources["Window"];
        public static object WindowActiveBorder => Brushes.Resources["WindowActiveBorder"];
        public static object WindowButtonMouseOverBackground => Brushes.Resources["WindowButtonMouseOverBackground"];
        public static object WindowButtonMouseOverGlyph => Brushes.Resources["WindowButtonMouseOverGlyph"];
        static Brushes() {
            Brushes.Resources = (ResourceDictionary)Application.LoadComponent(new Uri("/Circus.Wpf;component/Themes/Light.xaml", UriKind.Relative));

            // Put this in theme initializer instead !!
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            AppContext.SetSwitch("Switch.System.Windows.Controls.Text.UseAdornerForTextboxSelectionRendering", false);
            ResourceManager.Add("Circus.Wpf", new ResourceLocation(Image.Alias, "Images"));
        }
        private Brushes() {
        }
        public static SolidColorBrush GetBrush(object brush) {
            return (SolidColorBrush)brush;
        }
    }
}
