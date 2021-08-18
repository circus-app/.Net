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
// A thumb for a headered control.
//
// A canvas that uses a fill brush to draw an horizontal grip. This is
// exclusively visual since headered controls and not draggable.


#pragma warning disable IDE0002

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Circus.Wpf.Controls {
    /// <summary>Provides a thumb for a headered control.</summary>
    public class HeaderedControlThumb : Canvas {
        /// <summary>Identifies the foreground dependency property.</summary>
        public static readonly DependencyProperty ForegroundProperty;
        /// <summary>Gets or sets the control foreground.</summary>
        public Brush Foreground { get => (Brush)this.GetValue(HeaderedControlThumb.ForegroundProperty); set => this.SetValue(HeaderedControlThumb.ForegroundProperty, value); }
        static HeaderedControlThumb() {
            HeaderedControlThumb.ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(HeaderedControlThumb), new FrameworkPropertyMetadata(null));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderedControlThumb), new FrameworkPropertyMetadata(typeof(HeaderedControlThumb)));
        }
        public HeaderedControlThumb() {
        }
    }
}
