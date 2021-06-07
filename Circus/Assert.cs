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
// Some assertion methods.
//
// Methods with an ouput parameter ensure that the output value is not 
// null. Nullity includes empty strings.
//
// As<T> is intended for cases where the type is known in advance and
// therefore does not require validation.


#pragma warning disable IDE0002

using System;
using Circus.Runtime;
namespace Circus {
    /// <summary>Provides a set of assertion methods.</summary>
    public sealed class Assert {
        private Assert() {
        }
        /// <summary>Converts the provided object to the specified T type and outputs the resulting T value. Returns true if obj is not null.</summary>
        public static bool As<T>(object obj, out T value) {
            return (Allocator.Assign(Assert.NotNull(obj), out bool num) & Allocator.Assign(num ? (T)obj : default, out value)) && num;
        }
        private static bool Cast<T>(object obj, out T value) {
            return (Allocator.Assign(Assert.Is<T>(obj), out bool num) & Allocator.Assign(num ? (T)obj : default, out value)) && num;
        }
        /// <summary>Determines if the provided object is of the specified T type.</summary>
        public static bool Is<T>(object obj) {
            return (obj is T) || Assert.Is(obj, typeof(T));
        }
        /// <summary>Determines if the provided object is of the specified type.</summary>
        public static bool Is(object obj, Type type) {
            return Allocator.Assign(obj.GetType(), out Type t) && (t.IsAssignableFrom(type) || t.IsSubclassOf(type));
        }
        /// <summary>Determines if the provided object is not null and of the specified T type. Outputs T if true, otherwise default.</summary>
        public static bool Is<T>(object obj, out T value) {
            if (Assert.Null(obj) && Allocator.Assign(null, out value)) {
                return false;
            }
            return Assert.Cast<T>(obj, out value);
        }
        /// <summary>Determines if the provided object is not null.</summary>
        public static bool NotNull(object obj) {
            return !Assert.Null(obj);
        }
        /// <summary>Determines if the provided object is null.</summary>
        public static bool Null(object obj) {
            return obj == null || (Assert.Cast(obj, out string value) && string.IsNullOrEmpty(value));
        }
    }
}