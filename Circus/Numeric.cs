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
// Some methods for numeric values and numeric types.
//
// Assertions use an internal set of numeric types to determine if the
// provided object or type matches an entry. Nullable types are supported.
//
// Assertion of string type is implemented in a core function that returns
// true if the value is a number and outputs flags to check if it's signed
// and/or a decimal. See Circus.Core/string.h for details.
//
// Cast methods do not check if the specified type is of a numeric type.
// This is to avoid checking multiple times the same value as they are 
// often called in loops or nested conditions. Use assertion prior to
// calling these methods if type validation is required.


#pragma warning disable IDE0002

using System;
using System.Runtime.InteropServices;
using System.Security;
using Circus.Collections;
using Circus.Runtime;
namespace Circus {
    /// <summary>Provides a set of methods for numeric values and numeric types.</summary>
    public sealed class Numeric {
        [ThreadStatic]
        private static readonly Set<Type> Types;
        static Numeric() {
            Numeric.Types = new Set<Type>(11) { typeof(byte), typeof(decimal), typeof(double), typeof(float), typeof(int), typeof(long), typeof(sbyte), typeof(short), typeof(uint), typeof(ulong), typeof(ushort) };
        }
        private Numeric() {
        }
        /// <summary>Converts the provided object to the T numeric type. Value and T must be of the same type.</summary>
        public static bool Cast<T>(object value, out T result) {
            return (Allocator.Assign(Numeric.Cast(value, typeof(T), out dynamic r), out bool num) & Allocator.Assign(num ? (T)r : default, out result)) && num;
        }
        /// <summary>Converts the provided object to the specified numeric type. Value and type must be of the same type.</summary>
        public static bool Cast(object value, Type type, out dynamic result) {
            return Allocator.Assign(value.GetType() == type ? Convert.ChangeType(value, type) : null, out result) && Circus.Assert.NotNull(result);
        }
        /// <summary>Returns true if the provided type is of a numeric type.</summary>
        public static bool Is(Type type) {
            return Allocator.Assign(Nullable.GetUnderlyingType(type), out Type t) && (Numeric.Types.Contains(type) || (Assert.NotNull(t) && Numeric.Is(t)));
        }
        /// <summary>Returns true if the provided object is of a numeric type.</summary>
        public static bool Is(object obj) {
            return Assert.Is(obj, out string value) ? Numeric.Is(value) : Numeric.Is(obj.GetType());
        }
        /// <summary>Returns true if the provided string can convert to a numeric type.</summary>
        public static bool Is(string value) {
            return Numeric.Is(value, out _, out _);
        }
        /// <summary>Returns true if the provided string can convert to a numeric type. Outputs if it's signed and if it's a decimal.</summary>
        [SecuritySafeCritical]
        public static unsafe bool Is(string value, out bool signed, out bool _decimal) {
            fixed (char* ptr = value) {
                return Numeric.IsNumeric(ptr, value.Length, out signed, out _decimal);
            }
        }
        /// <summary>Determines if the provided value is a prime.</summary>
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Circus.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsPrime(int value);
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Circus.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe bool IsNumeric(char* value, int size, out bool signed, out bool _decimal);
        /// <summary>Returns the next prime equal or greater than the provided value.</summary>
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Circus.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NextPrime(int value);
        /// <summary>Returns the previous prime equal or smaller than the provided value.</summary>
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Circus.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PreviousPrime(int value);
    }
}
