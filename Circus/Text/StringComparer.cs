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
// A string comparer that uses a Boyer-Moore algorythm for equality.
//
// Based on StringInfo.Equals for equality comparison.
//
// GetHashCode() returns the .net hash code as an integer since it is part
// of the IEqualityComparer interface. Use StringInfo.GetHash() instead if 
// Farmhash is preferred.


using System.Collections.Generic;
namespace Circus.Text {
    /// <summary>Provides a string comparer that uses a Boyer-Moore algorythm for equality.</summary>
    public sealed class StringComparer : IEqualityComparer<string> {
        /// <summary>Constructs a string comprarer.</summary>
        public StringComparer() { 
        }
        bool IEqualityComparer<string>.Equals(string x, string y) {
            return StringInfo.Equals(x, y);
        }
        int IEqualityComparer<string>.GetHashCode(string obj) {
            return obj.GetHashCode();
        }
    }
}