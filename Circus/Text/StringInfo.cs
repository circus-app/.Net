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
// Fast and efficient functionalities for strings.


#pragma warning disable IDE0002

using System.Runtime.InteropServices;
using System.Security;
using Circus.Runtime;
namespace Circus.Text {
    /// <summary>Provides fast and efficient functionalities for strings.</summary>
    public sealed class StringInfo {
        private StringInfo() { 
        }
        /// <summary>Determines if the provided source string contains the specified value string. Outputs the index of the first occurrence. Returns true if found.</summary>
        [SecuritySafeCritical]
        public static unsafe bool Contains(string source, string value, out int index) {
            fixed (char* ptr = source) {
                fixed (char* ptr2 = value) {
                    return Allocator.Assign(StringInfo.Contains(ptr, source.Length, ptr2, value.Length), out index) && index > - 1;
                }
            }
        }
        /// <summary>Determines if the provided x and y strings are equal.</summary>
        [SecuritySafeCritical]
        public static unsafe bool Equals(string x, string y) {
            fixed (char* ptr = x) {
                fixed (char* ptr2 = y) {
                    return StringInfo.Equals(ptr, x.Length, ptr2, y.Length);
                }
            }
        }
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Circus.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe bool Equals(char* str, int n, char* str1, int n1);
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Circus.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int Contains(char* str, int n, char* str1, int n1);
        /// <summary>Searches the provided source string for the first character that does not match any of the characters specified in value. Outputs the index of the first occurrence. Returns true if found.</summary>
        [SecuritySafeCritical]
        public static unsafe bool FirstNotOf(string source, string value, out int index) {
            fixed (char* ptr = source) {
                fixed (char* ptr2 = value) {
                    return Allocator.Assign(StringInfo.FirstNotOf(ptr, source.Length, ptr2, value.Length), out index) && index > -1;
                }
            }
        }
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Circus.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int FirstNotOf(char* str, int n, char* str1, int n1);
        /// <summary>Returns the hash code of the specified string as an unsigned 64-bit integer using Google's Farmhash algorythm.</summary>
        [SecuritySafeCritical]
        public static unsafe ulong GetHash(string value) {
            fixed (char* ptr = value) {
                return StringInfo.GetHash(ptr, value.Length, out ulong hash) ? hash : (ulong)value.GetHashCode();
            }
        }
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Circus.Core.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Hash")]
        private static extern unsafe bool GetHash(char* str, int n, out ulong hash);
        /// <summary>Determines if the provided source string contains the specified value string. Outputs the index of the last occurrence. Returns true if found.</summary>
        [SecuritySafeCritical]
        public static unsafe bool Last(string source, string value, out int index) {
            fixed (char* ptr = source) {
                fixed (char* ptr2 = value) {
                    return Allocator.Assign(StringInfo.Last(ptr, source.Length, ptr2, value.Length), out index) && index > -1;
                }
            }
        }
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Circus.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int Last(char* str, int n, char* str1, int n1);
        /// <summary>Searches the provided source string for the first character that does not match any of the characters specified in value. Outputs the index of the last occurrence. Returns true if found.</summary>
        [SecuritySafeCritical]
        public static unsafe bool LastNotOf(string source, string value, out int index) {
            fixed (char* ptr = source) {
                fixed (char* ptr2 = value) {
                    return Allocator.Assign(StringInfo.LastNotOf(ptr, source.Length, ptr2, value.Length), out index) && index > -1;
                }
            }
        }
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Circus.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int LastNotOf(char* str, int n, char* str1, int n1);
    }
}
