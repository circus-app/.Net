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
// A component to dynamically register template parts at runtime.
//
// Registering is achieved by scanning control properties that matches a class
// defined TemplatePartAttribute and setting their value to a corresponding named
// item within the control template. It must be called in the OnApplyTemplate()
// method of the control providing an instance reference of itself:
//
// public override void OnApplyTemplate() {
//      TemplateItems.Register(this);
// }
//
// Properties can be public, internal or private. Be aware that base private
// properties are not included in the search.
//
// The registering only works with items derived from Control since it is
// the first element in the visual hierarchy that implements ControlTemplate.
//
// FindVisualChild returns an object instead of a dependency object to allow
// searching by interface (i.e. ITransaction, etc.).


#pragma warning disable IDE0002

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Circus.Wpf {
    /// <summary>Provides a component to dynamically register template parts at runtime.</summary>
    public sealed class TemplateItems {
        private TemplateItems() { 
        }
        /// <summary>Outputs the template part with the specified name within the provided control. Returns true if found, otherwise false.</summary>
        public static bool FindName(Control control, string name, out object value) {
            value = control.Template.FindName(name, control);
            return Assert.NotNull(value);
        }
        /// <summary>Outputs the first visual child that matches the specified type T within the provided dependency object. Returns true if found, otherwise false.</summary>
        public static bool FindVisualChild<T>(DependencyObject d, out object value) {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++) {
                DependencyObject o = VisualTreeHelper.GetChild(d, i);
                if (Assert.NotNull(o) && Assert.Is<T>(o)) {
                    value = o;
                    return true;
                }
                if (TemplateItems.FindVisualChild<T>(o, out value) && Assert.NotNull(value)) {
                    return true;
                }
            }
            value = null;
            return false;
        } 
        /// <summary>Registers template parts that are defined in the specified control.</summary>
        public static void Register(Control control) {
            Type type = control.GetType();
            IEnumerable<TemplatePartAttribute> array = type.GetCustomAttributes<TemplatePartAttribute>(true);
            foreach (TemplatePartAttribute attribute in array) {
                PropertyInfo property = type.GetProperty(attribute.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (Assert.NotNull(property) && property.PropertyType == attribute.Type && TemplateItems.FindName(control, attribute.Name, out object value)) {
                    property.SetValue(control, value);
                }
            }
        }
    }
}