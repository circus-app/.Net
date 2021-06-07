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
// Farmhash copyright notice:
// Copyright (c) 2014 Google, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


#pragma once

#define bswap_64(x) _byteswap_uint64(x)
#define uint32_in_expected_order(x) (x)
#define uint64_in_expected_order(x) (x)

#include <assert.h>
#include <string.h>
#include <utility>

namespace circus {

    namespace farmhash {

        using namespace std;
        namespace detail {

            typedef std::pair<uint64_t, uint64_t> uint128_t;

            static const uint64_t k0 = 0xc3a5c85c97cb3127ULL;
            static const uint64_t k1 = 0xb492b66fbe98f273ULL;
            static const uint64_t k2 = 0x9ae16a3b2f90404fULL;

            inline uint64_t Uint128Low64(const uint128_t x) {
                return x.first;
            }

            inline uint64_t Uint128High64(const uint128_t x) {
                return x.second;
            }

            inline uint128_t Uint128(uint64_t lo, uint64_t hi) {
                return uint128_t(lo, hi);
            }

            inline uint64_t Hash128to64(uint128_t x) {
                const uint64_t kMul = 0x9ddfea08eb382d69ULL;
                uint64_t a = (Uint128Low64(x) ^ Uint128High64(x)) * kMul;
                a ^= (a >> 47);
                uint64_t b = (Uint128High64(x) ^ a) * kMul;
                b ^= (b >> 47);
                b *= kMul;
                return b;
            }

            static inline uint64_t BasicRotate64(uint64_t val, int shift) {
                return shift == 0 ? val : ((val >> shift) | (val << (64 - shift)));
            }

            static inline uint64_t Bswap64(uint64_t val) {
                return bswap_64(val);
            }

            static inline uint32_t Fetch32(const char* p) {
                uint32_t result;
                memcpy(&result, p, sizeof(result));
                return uint32_in_expected_order(result);
            }

            static inline uint64_t Fetch64(const char* p) {
                uint64_t result;
                memcpy(&result, p, sizeof(result));
                return uint64_in_expected_order(result);
            }

            static inline uint64_t Rotate64(uint64_t val, int shift) {
                return BasicRotate64(val, shift);
            }

        }  // namespace details

        using namespace detail;
        namespace farmn {

        #undef Fetch
        #define Fetch Fetch64
        #undef Rotate
        #define Rotate Rotate64
        #undef Bswap
        #define Bswap Bswap64

            static inline uint64_t ShiftMix(uint64_t val) {
                return val ^ (val >> 47);
            }

            static inline uint64_t HashLen16(uint64_t u, uint64_t v) {
                return Hash128to64(Uint128(u, v));
            }

            static inline uint64_t HashLen16(uint64_t u, uint64_t v, uint64_t mul) {
                uint64_t a = (u ^ v) * mul;
                a ^= (a >> 47);
                uint64_t b = (v ^ a) * mul;
                b ^= (b >> 47);
                b *= mul;
                return b;
            }

            static inline uint64_t HashLen0to16(const char* s, size_t len) {
                if (len >= 8) {
                    uint64_t mul = k2 + len * 2;
                    uint64_t a = Fetch(s) + k2;
                    uint64_t b = Fetch(s + len - 8);
                    uint64_t c = Rotate(b, 37) * mul + a;
                    uint64_t d = (Rotate(a, 25) + b) * mul;
                    return HashLen16(c, d, mul);
                }
                if (len >= 4) {
                    uint64_t mul = k2 + len * 2;
                    uint64_t a = farmhash::Fetch32(s);
                    return HashLen16(len + (a << 3), farmhash::Fetch32(s + len - 4), mul);
                }
                if (len > 0) {
                    uint8_t a = s[0];
                    uint8_t b = s[len >> 1];
                    uint8_t c = s[len - 1];
                    uint32_t y = static_cast<uint32_t>(a) + (static_cast<uint32_t>(b) << 8);
                    uint32_t z = len + (static_cast<uint32_t>(c) << 2);
                    return ShiftMix(y * k2 ^ z * k0) * k2;
                }
                return k2;
            }

            static inline uint64_t HashLen17to32(const char* s, size_t len) {
                uint64_t mul = k2 + len * 2;
                uint64_t a = Fetch(s) * k1;
                uint64_t b = Fetch(s + 8);
                uint64_t c = Fetch(s + len - 8) * mul;
                uint64_t d = Fetch(s + len - 16) * k2;
                return HashLen16(Rotate(a + b, 43) + Rotate(c, 30) + d, a + Rotate(b + k2, 18) + c, mul);
            }

            static inline pair<uint64_t, uint64_t> WeakHashLen32WithSeeds(uint64_t w, uint64_t x, uint64_t y, uint64_t z, uint64_t a, uint64_t b) {
                a += w;
                b = Rotate(b + a + z, 21);
                uint64_t c = a;
                a += x;
                a += y;
                b += Rotate(a, 44);
                return make_pair(a + z, b + c);
            }

            static inline pair<uint64_t, uint64_t> WeakHashLen32WithSeeds(const char* s, uint64_t a, uint64_t b) {
                return WeakHashLen32WithSeeds(Fetch(s), Fetch(s + 8), Fetch(s + 16), Fetch(s + 24), a, b);
            }

            static inline uint64_t HashLen33to64(const char* s, size_t len) {
                uint64_t mul = k2 + len * 2;
                uint64_t a = Fetch(s) * k2;
                uint64_t b = Fetch(s + 8);
                uint64_t c = Fetch(s + len - 8) * mul;
                uint64_t d = Fetch(s + len - 16) * k2;
                uint64_t y = Rotate(a + b, 43) + Rotate(c, 30) + d;
                uint64_t z = HashLen16(y, a + Rotate(b + k2, 18) + c, mul);
                uint64_t e = Fetch(s + 16) * mul;
                uint64_t f = Fetch(s + 24);
                uint64_t g = (y + Fetch(s + len - 32)) * mul;
                uint64_t h = (z + Fetch(s + len - 24)) * mul;
                return HashLen16(Rotate(e + f, 43) + Rotate(g, 30) + h, e + Rotate(f + a, 18) + g, mul);
            }

            uint64_t Hash64(const char* s, size_t len) {
                const uint64_t seed = 81;
                if (len <= 32) {
                    if (len <= 16) {
                        return HashLen0to16(s, len);
                    }
                    else {
                        return HashLen17to32(s, len);
                    }
                }
                else if (len <= 64) {
                    return HashLen33to64(s, len);
                }

                // For strings over 64 bytes we loop. Internal state consists of 56 bytes: v, w, x, y, and z.
                uint64_t x = seed;
                uint64_t y = seed * k1 + 113;
                uint64_t z = ShiftMix(y * k2 + 113) * k2;
                pair<uint64_t, uint64_t> v = make_pair(0, 0);
                pair<uint64_t, uint64_t> w = make_pair(0, 0);
                x = x * k2 + Fetch(s);

                // Set end so that after the loop we have 1 to 64 bytes left to process.
                const char* end = s + ((len - 1) / 64) * 64;
                const char* last64 = end + ((len - 1) & 63) - 63;
                assert(s + len - 64 == last64);
                do {
                    x = Rotate(x + y + v.first + Fetch(s + 8), 37) * k1;
                    y = Rotate(y + v.second + Fetch(s + 48), 42) * k1;
                    x ^= w.second;
                    y += v.first + Fetch(s + 40);
                    z = Rotate(z + w.first, 33) * k1;
                    v = WeakHashLen32WithSeeds(s, v.second * k1, x + w.first);
                    w = WeakHashLen32WithSeeds(s + 32, z + w.second, y + Fetch(s + 16));
                    swap(z, x);
                    s += 64;
                } while (s != end);
                uint64_t mul = k1 + ((z & 0xff) << 1);

                // Make s point to the last 64 bytes of input.
                s = last64;
                w.first += ((len - 1) & 63);
                v.first += w.first;
                w.first += v.first;
                x = Rotate(x + y + v.first + Fetch(s + 8), 37) * mul;
                y = Rotate(y + v.second + Fetch(s + 48), 42) * mul;
                x ^= w.second * 9;
                y += v.first * 9 + Fetch(s + 40);
                z = Rotate(z + w.first, 33) * mul;
                v = WeakHashLen32WithSeeds(s, v.second * mul, x + w.first);
                w = WeakHashLen32WithSeeds(s + 32, z + w.second, y + Fetch(s + 16));
                swap(z, x);
                return HashLen16(HashLen16(v.first, w.first, mul) + ShiftMix(y) * k0 + z, HashLen16(v.second, w.second, mul) + x, mul);
            }

            uint64_t Hash64WithSeeds(const char* s, size_t len, uint64_t seed0, uint64_t seed1);

            uint64_t Hash64WithSeed(const char* s, size_t len, uint64_t seed) {
                return Hash64WithSeeds(s, len, k2, seed);
            }

            uint64_t Hash64WithSeeds(const char* s, size_t len, uint64_t seed0, uint64_t seed1) {
                return HashLen16(Hash64(s, len) - seed0, seed1);
            }

        }  // namespace farmn

        namespace farmu {

        #undef Fetch
        #define Fetch Fetch64
        #undef Rotate
        #define Rotate Rotate64

            static inline uint64_t H(uint64_t x, uint64_t y, uint64_t mul, int r) {
                uint64_t a = (x ^ y) * mul;
                a ^= (a >> 47);
                uint64_t b = (y ^ a) * mul;
                return Rotate(b, r) * mul;
            }

            uint64_t Hash64WithSeeds(const char* s, size_t len, uint64_t seed0, uint64_t seed1) {
                if (len <= 64) {
                    return farmn::Hash64WithSeeds(s, len, seed0, seed1);
                }

                // For strings over 64 bytes we loop. Internal state consists of 64 bytes: u, v, w, x, y, and z.
                uint64_t x = seed0;
                uint64_t y = seed1 * k2 + 113;
                uint64_t z = farmn::ShiftMix(y * k2) * k2;
                pair<uint64_t, uint64_t> v = make_pair(seed0, seed1);
                pair<uint64_t, uint64_t> w = make_pair(0, 0);
                uint64_t u = x - z;
                x *= k2;
                uint64_t mul = k2 + (u & 0x82);

                // Set end so that after the loop we have 1 to 64 bytes left to process.
                const char* end = s + ((len - 1) / 64) * 64;
                const char* last64 = end + ((len - 1) & 63) - 63;
                assert(s + len - 64 == last64);
                do {
                    uint64_t a0 = Fetch(s);
                    uint64_t a1 = Fetch(s + 8);
                    uint64_t a2 = Fetch(s + 16);
                    uint64_t a3 = Fetch(s + 24);
                    uint64_t a4 = Fetch(s + 32);
                    uint64_t a5 = Fetch(s + 40);
                    uint64_t a6 = Fetch(s + 48);
                    uint64_t a7 = Fetch(s + 56);
                    x += a0 + a1;
                    y += a2;
                    z += a3;
                    v.first += a4;
                    v.second += a5 + a1;
                    w.first += a6;
                    w.second += a7;

                    x = Rotate(x, 26);
                    x *= 9;
                    y = Rotate(y, 29);
                    z *= mul;
                    v.first = Rotate(v.first, 33);
                    v.second = Rotate(v.second, 30);
                    w.first ^= x;
                    w.first *= 9;
                    z = Rotate(z, 32);
                    z += w.second;
                    w.second += z;
                    z *= 9;
                    swap(u, y);

                    z += a0 + a6;
                    v.first += a2;
                    v.second += a3;
                    w.first += a4;
                    w.second += a5 + a6;
                    x += a1;
                    y += a7;

                    y += v.first;
                    v.first += x - y;
                    v.second += w.first;
                    w.first += v.second;
                    w.second += x - y;
                    x += w.second;
                    w.second = Rotate(w.second, 34);
                    swap(u, z);
                    s += 64;
                } while (s != end);

                // Make s point to the last 64 bytes of input.
                s = last64;
                u *= 9;
                v.second = Rotate(v.second, 28);
                v.first = Rotate(v.first, 20);
                w.first += ((len - 1) & 63);
                u += y;
                y += u;
                x = Rotate(y - x + v.first + Fetch(s + 8), 37) * mul;
                y = Rotate(y ^ v.second ^ Fetch(s + 48), 42) * mul;
                x ^= w.second * 9;
                y += v.first + Fetch(s + 40);
                z = Rotate(z + w.first, 33) * mul;
                v = farmn::WeakHashLen32WithSeeds(s, v.second * mul, x + w.first);
                w = farmn::WeakHashLen32WithSeeds(s + 32, z + w.second, y + Fetch(s + 16));
                return H(farmn::HashLen16(v.first + x, w.first ^ y, mul) + z - u, H(v.second + y, w.second + z, k2, 30) ^ x, k2, 31);
            }

            uint64_t Hash64WithSeed(const char* s, size_t len, uint64_t seed) {
                return len <= 64 ? farmn::Hash64WithSeed(s, len, seed) : Hash64WithSeeds(s, len, 0, seed);
            }

            uint64_t Hash64(const char* s, size_t len) {
                return len <= 64 ? farmn::Hash64(s, len) : Hash64WithSeeds(s, len, 81, 0);
            }

        }  // namespace farmu

        namespace farmx {

        #undef Fetch
        #define Fetch Fetch64
        #undef Rotate
        #define Rotate Rotate64

            static inline uint64_t H32(const char* s, size_t len, uint64_t mul, uint64_t seed0 = 0, uint64_t seed1 = 0) {
                uint64_t a = Fetch(s) * k1;
                uint64_t b = Fetch(s + 8);
                uint64_t c = Fetch(s + len - 8) * mul;
                uint64_t d = Fetch(s + len - 16) * k2;
                uint64_t u = Rotate(a + b, 43) + Rotate(c, 30) + d + seed0;
                uint64_t v = a + Rotate(b + k2, 18) + c + seed1;
                a = farmn::ShiftMix((u ^ v) * mul);
                b = farmn::ShiftMix((v ^ a) * mul);
                return b;
            }

            static inline uint64_t HashLen33to64(const char* s, size_t len) {
                uint64_t mul0 = k2 - 30;
                uint64_t mul1 = k2 - 30 + 2 * len;
                uint64_t h0 = H32(s, 32, mul0);
                uint64_t h1 = H32(s + len - 32, 32, mul1);
                return ((h1 * mul1) + h0) * mul1;
            }

            static inline uint64_t HashLen65to96(const char* s, size_t len) {
                uint64_t mul0 = k2 - 114;
                uint64_t mul1 = k2 - 114 + 2 * len;
                uint64_t h0 = H32(s, 32, mul0);
                uint64_t h1 = H32(s + 32, 32, mul1);
                uint64_t h2 = H32(s + len - 32, 32, mul1, h0, h1);
                return (h2 * 9 + (h0 >> 17) + (h1 >> 21))* mul1;
            }

            uint64_t Hash64(const char* s, size_t len) {
                if (len <= 32) {
                    if (len <= 16) {
                        return farmn::HashLen0to16(s, len);
                    }
                    else {
                        return farmn::HashLen17to32(s, len);
                    }
                }
                else if (len <= 64) {
                    return HashLen33to64(s, len);
                }
                else if (len <= 96) {
                    return HashLen65to96(s, len);
                }
                else if (len <= 256) {
                    return farmn::Hash64(s, len);
                }
                else {
                    return farmu::Hash64(s, len);
                }
            }

            uint64_t Hash64WithSeeds(const char* s, size_t len, uint64_t seed0, uint64_t seed1) {
                return farmu::Hash64WithSeeds(s, len, seed0, seed1);
            }

            uint64_t Hash64WithSeed(const char* s, size_t len, uint64_t seed) {
                return farmu::Hash64WithSeed(s, len, seed);
            }

        }  // namespace farmx

    } // namespace farmhash

} // namespace circus
