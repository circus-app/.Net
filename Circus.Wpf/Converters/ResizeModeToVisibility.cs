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


using System;
using System.Globalization;
using System.Windows;
namespace Circus.Wpf.Converters {
    internal sealed class ResizeModeToVisibility : Converter {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return !(value is ResizeMode mode) || mode != ResizeMode.NoResize ? Visibility.Visible : Visibility.Collapsed;
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return !(value is Visibility visibility) || visibility == Visibility.Visible ? ResizeMode.CanResize : ResizeMode.NoResize;
        }
    }
}
