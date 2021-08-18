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
// Value extensions for several types.


namespace Circus.Wpf {
    /// <summary>Provides a markup extension for a boolean value.</summary>
    public sealed class BoolExtension : ValueExtension<bool> {
        /// <summary>Constructs a boolean with the specified value.</summary>
        public BoolExtension(bool value) : base(value) {
        }
    }
    /// <summary>Provides a markup extension for a byte value.</summary>
    public sealed class ByteExtension : ValueExtension<byte> {
        /// <summary>Constructs a byte with the specified value.</summary>
        public ByteExtension(byte value) : base(value) { 
        }
    }
    /// <summary>Provides a markup extension for a char value.</summary>
    public sealed class CharExtension : ValueExtension<char> {
        /// <summary>Constructs a char with the specified value.</summary>
        public CharExtension(char value) : base(value) {
        }
    }
    /// <summary>Provides a markup extension for a decimal value.</summary>
    public sealed class DecimalExtension : ValueExtension<decimal> {
        /// <summary>Constructs a decimal with the specified value.</summary>
        public DecimalExtension(decimal value) : base(value) {
        }
    }
    /// <summary>Provides a markup extension for a double value.</summary>
    public sealed class DoubleExtension : ValueExtension<double> {
        /// <summary>Constructs a double with the specified value.</summary>
        public DoubleExtension(double value) : base(value) {
        }
    }
    /// <summary>Provides a markup extension for a float value.</summary>
    public sealed class FloatExtension : ValueExtension<float> {
        /// <summary>Constructs a float with the specified value.</summary>
        public FloatExtension(float value) : base(value) {
        }
    }
    /// <summary>Provides a markup extension for an integer value.</summary>
    public sealed class IntExtension : ValueExtension<int> {
        /// <summary>Constructs an integer with the specified value.</summary>
        public IntExtension(int value) : base(value) {
        }
    }
    /// <summary>Provides a markup extension for a long value.</summary>
    public sealed class LongExtension : ValueExtension<long> {
        /// <summary>Constructs a long with the specified value.</summary>
        public LongExtension(long value) : base(value) {
        }
    }
    /// <summary>Provides a markup extension for an sbyte value.</summary>
    public sealed class SByteExtension : ValueExtension<sbyte> {
        /// <summary>Constructs an sbyte with the specified value.</summary>
        public SByteExtension(sbyte value) : base(value) {
        }
    }
    /// <summary>Provides a markup extension for a short value.</summary>
    public sealed class ShortExtension : ValueExtension<short> {
        /// <summary>Constructs a short with the specified value.</summary>
        public ShortExtension(short value) : base(value) {
        }
    }
    /// <summary>Provides a markup extension for a string value.</summary>
    public sealed class StringExtension : ValueExtension<string> {
        /// <summary>Constructs a string with the specified value.</summary>
        public StringExtension(string value) : base(value) {
        }
    }
    /// <summary>Provides a markup extension for an unsigned integer value.</summary>
    public sealed class UIntExtension : ValueExtension<uint> {
        /// <summary>Constructs an unsigned integer with the specified value.</summary>
        public UIntExtension(uint value) : base(value) {
        }
    }
    /// <summary>Provides a markup extension for an unsigned long value.</summary>
    public sealed class ULongExtension : ValueExtension<ulong> {
        /// <summary>Constructs an unsigned long with the specified value.</summary>
        public ULongExtension(ulong value) : base(value) {
        }
    }
    /// <summary>Provides a markup extension for an unsigned short value.</summary>
    public sealed class UShortExtension : ValueExtension<ushort> {
        /// <summary>Constructs an unsigned short with the specified value.</summary>
        public UShortExtension(ushort value) : base(value) {
        }
    }
}
