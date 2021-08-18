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
// A multi-value converter of boolean values.
//
// It provides an optional operator that defines the boolean condition that
// is performed. The default value is And.
//
// This is intended for MultiDataTrigger.Conditions.
//
// Converter parameter is ignored.


#pragma warning disable IDE0002

using System;
using System.Globalization;
using Circus.Runtime;

namespace Circus.Wpf.Converters {
    /// <summary>Provides a multi-value converter of boolean values.</summary>
    public class BooleanSwitch : MultiValue<bool> {
        /// <summary>Gets or sets the boolean operator to be used. Default value is And.</summary>
        public BooleanOperator Operator { get => base.GetValue<BooleanOperator>(BooleanOperator.And); set => base.SetValue(value); }
        /// <summary>Constructs a BooleanSwitch converter.</summary>
        public BooleanSwitch() { 
        }
        /// <summary>Constructs a BooleanSwitch converter with the specified boolean operator.</summary>
        public BooleanSwitch(BooleanOperator value) {
            this.Operator = value;
        }
        private static bool Convert(object[] array, bool flag) {
            if (Allocator.Assign(flag, out bool num)) {
                for (int i = 0; i < array.Length; i++) {
                    if (!Assert.Is(array[i], out bool value)) {
                        return false;
                    }
                    num = flag ? num && value : num || value;
                }
                return num;
            }
            return false;
        }
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            return BooleanSwitch.Convert(values, this.Operator == BooleanOperator.And);
        }
    }
}
