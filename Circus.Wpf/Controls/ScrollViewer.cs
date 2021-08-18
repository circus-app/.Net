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
// A themed scroll viewer.


#pragma warning disable IDE0002

using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a themed scroll viewer.</summary>
    public class ScrollViewer : System.Windows.Controls.ScrollViewer {
        /// <summary>Returns the resource key for an horizontal scroll bar template.</summary>
        public static readonly ResourceKey ScrollBarHorizontalTemplateKey;
        /// <summary>Returns the resource key for a scroll bar line button style.</summary>
        public static readonly ResourceKey ScrollBarLineButtonStyleKey;
        /// <summary>Returns the resource key for a scroll bar page button style.</summary>
        public static readonly ResourceKey ScrollBarPageButtonStyleKey;
        /// <summary>Returns the resource key for a scroll bar style.</summary>
        public static readonly ResourceKey ScrollBarStyleKey;
        /// <summary>Returns the resource key for a scroll bar generic thumb style.</summary>
        public static readonly ResourceKey ScrollBarThumbStyleKey;
        /// <summary>Returns the resource key for a scroll bar vertical thumb style.</summary>
        public static readonly ResourceKey ScrollBarThumbVerticalStyleKey;
        /// <summary>Returns the resource key for a scroll bar horizontal thumb style.</summary>
        public static readonly ResourceKey ScrollBarThumbHorizontalStyleKey;
        /// <summary>Returns the resource key for a vertical scroll bar template.</summary>
        public static readonly ResourceKey ScrollBarVerticalTemplateKey;
        /// <summary>Identifies the scroll bars background dependency property.</summary>
        public static readonly DependencyProperty ScrollBarBackgroundProperty;
        /// <summary>Gets or sets a brush that describes the background of the scroll bars.</summary>
        [Bindable(true)]
        public Brush ScrollBarBackground { get => (Brush)this.GetValue(ScrollViewer.ScrollBarBackgroundProperty); set => this.SetValue(ScrollViewer.ScrollBarBackgroundProperty, value); }
        static ScrollViewer() {
            ScrollViewer.ScrollBarHorizontalTemplateKey = new ComponentResourceKey(typeof(ScrollViewer), "ScrollBarHorizontalTemplateKey");
            ScrollViewer.ScrollBarLineButtonStyleKey = new ComponentResourceKey(typeof(ScrollViewer), "ScrollBarLineButtonStyleKey");
            ScrollViewer.ScrollBarPageButtonStyleKey = new ComponentResourceKey(typeof(ScrollViewer), "ScrollBarPageButtonStyleKey");
            ScrollViewer.ScrollBarStyleKey = new ComponentResourceKey(typeof(ScrollViewer), "ScrollBarStyleKey");
            ScrollViewer.ScrollBarThumbStyleKey = new ComponentResourceKey(typeof(ScrollViewer), "ScrollBarThumbStyleKey");
            ScrollViewer.ScrollBarThumbVerticalStyleKey = new ComponentResourceKey(typeof(ScrollViewer), "ScrollBarThumbVerticalStyleKey");
            ScrollViewer.ScrollBarThumbHorizontalStyleKey = new ComponentResourceKey(typeof(ScrollViewer), "ScrollBarThumbHorizontalStyleKey");
            ScrollViewer.ScrollBarVerticalTemplateKey = new ComponentResourceKey(typeof(ScrollViewer), "ScrollBarVerticalTemplateKey");
            ScrollViewer.ScrollBarBackgroundProperty = DependencyProperty.Register("ScrollBarBackground", typeof(Brush), typeof(ScrollViewer), new FrameworkPropertyMetadata(Themes.Brushes.ControlBackground));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollViewer), new FrameworkPropertyMetadata(typeof(ScrollViewer)));
        }
        /// <summary>Constructs a themed scroll viewer.</summary>
        public ScrollViewer() { 
        }
    }
}
