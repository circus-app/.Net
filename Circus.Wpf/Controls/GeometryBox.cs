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
// A scalable decorator that contains cached GeometryDrawing elements.
//
// The purpose was to have a presenter filled with vector drawings that 
// provides different visual aspects whether its parent control is
// enabled or not. A typical use is a button icon.
//
// Since it inherits from Viewbox, the content is a single child that 
// matches the following hierarchy:
//
// <Rectangle Height="SomeHeight" Width="SomeWidth">
//     <Rectangle.Fill>
//         <DrawingBrush>
//             <DrawingBrush.Drawing>
//                 <DrawingGroup>
//                     <DrawingGroup.Children>
//                         <GeometryDrawing> ...
//
// GeometryDrawing that should change aspect must declare the
// RenderOptions.CachingHint="Cache" attribute.
//
// The component looks for flagged GeometryDrawings whenever availability
// changes and creates a cache of their normal state brushes. When IsEnabled 
// is false, cached brushes are toggled with the GeometryBoxDisabled brush 
// defined in the active theme. 


#pragma warning disable IDE0002

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Circus.Collections;
namespace Circus.Wpf.Controls {
    /// <summary>Defines a scalable decorator that contains cached GeometryDrawing elements.</summary>
    [Serializable]
    public class GeometryBox : Viewbox, IResource {
        private struct Entry {
            internal Brush Brush;
            internal GeometryDrawing Geometry;
            internal Entry(GeometryDrawing geometry) {
                this.Brush = geometry.Brush;
                this.Geometry = geometry;
            }
        }
        private Pail<Entry> array;
        public string Id { get; set; }
        static GeometryBox() {
            GeometryBox.IsEnabledProperty.OverrideMetadata(typeof(GeometryBox), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(GeometryBox.OnIsEnabledChanged)));
        }
        public GeometryBox() {
        }
        private static bool GetDrawings(GeometryBox d, out DrawingCollection array) {
            Rectangle r = (Rectangle)d.Child;
            array = Assert.NotNull(r) && Assert.Is(r.Fill, out DrawingBrush brush) && Assert.Is(brush.Drawing, out DrawingGroup group) ? group.Children : null;
            return Assert.NotNull(array);
        }
        private bool GetEntry(Drawing d, out Entry entry) {
            bool num = RenderOptions.GetCachingHint(d) == CachingHint.Cache;
            entry = num ? new Entry((GeometryDrawing)d) : default;
            return num;
        }
        private bool IsCached() {
            if (Assert.NotNull(this.array)) {
                return true;
            }
            if (GeometryBox.GetDrawings(this, out DrawingCollection array)) {
                this.array = Pail<Entry>.Create<Drawing>(array, this.GetEntry);
            }
            return Assert.NotNull(this.array);
        }
        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            GeometryBox box = (GeometryBox)d;
            if (!box.IsCached()) {
                return;
            }
            if (Assert.Is(e.NewValue, out bool num)) {
                foreach (Entry entry in box.array) {
                    entry.Geometry.Brush = num ? entry.Brush : (Brush)Themes.Brushes.GeometryBoxDisabled;
                }
            }
        }
    }
}
