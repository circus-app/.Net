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
// Information and helper methods relative to the device screens.


#pragma warning disable IDE0002

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
namespace Circus.Wpf {
    /// <summary>Provides information and helper methods relative to the device screens.</summary>
    public sealed class Screen {
        /// <summary>Returns the width of a rectangle centered on a drag point to allow for limited movement of the mouse pointer before a drag operation begins.</summary>
        public static double MinimumHorizontalDragDistance { get; private set; }
        /// <summary>Returns the height of a rectangle centered on a drag point to allow for limited movement of the mouse pointer before a drag operation begins.</summary>
        public static double MinimumVerticalDragDistance { get; private set; }
        /// <summary>Returns the DPI scale of the X axis.</summary>
        public static double ScaleX { get; private set; }
        /// <summary>Returns the DPI scale of the Y axis.</summary>
        public static double ScaleY { get; private set; }
        static Screen() {
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
            Screen.Initialize(VisualTreeHelper.GetDpi(Application.Current.MainWindow));
        }
        private Screen() { 
        }
        /// <summary>Converts the specified size into a dpi aware size.</summary>
        public static Size Convert(Size size) {
            return new Size(size.Width * Screen.ScaleX, size.Height * Screen.ScaleY);
        }
        /// <summary>Returns the rectangle in screen coordinates that contains the specified UIElement using a dpi aware size.</summary>
        public static Rect GetBounds(UIElement element) {
            return new Rect(element.PointToScreen(new Point(0d, 0d)), Screen.Convert(element.RenderSize));
        }
        /// <summary>Returns the rectangle in screen coordinates that contains the specified Visual and optionally converts the provided size into a dpi aware size.</summary>
        public static Rect GetBounds(Visual visual, Size size, bool convert) {
            return new Rect(visual.PointToScreen(new Point(0d, 0d)), convert ? Screen.Convert(size) : size);
        }
        /// <summary>Outputs the size of the screen that contains the active window converted to a dpi aware size and using the specified options.</summary>
        public static bool GetSize(MonitorSize options, out Size size) {
            bool num = Screen.MonitorSize((int)options, out double width, out double height);
            size = new Size(width / Screen.ScaleX, height / Screen.ScaleY);
            return num;
        }
        private static void Initialize(DpiScale scale) {
            Screen.MinimumHorizontalDragDistance = SystemParameters.MinimumHorizontalDragDistance * scale.DpiScaleX;
            Screen.MinimumVerticalDragDistance = SystemParameters.MinimumVerticalDragDistance * scale.DpiScaleY;
            Screen.ScaleX = scale.DpiScaleX;
            Screen.ScaleY = scale.DpiScaleY;
        }
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Circus.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool MonitorSize(int flag, out double width, out double height);
        private static void OnDisplaySettingsChanged(object sender, EventArgs e) {
            Screen.Initialize(VisualTreeHelper.GetDpi(Application.Current.MainWindow));
        }
    }
}
