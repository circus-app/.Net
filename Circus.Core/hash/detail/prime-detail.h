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


#pragma once
#pragma warning(disable: 4267)

#include <windef.h>
#include <vector>

namespace circus {

    namespace prime {

	    namespace detail {

            // k0 and vector size are hardcoded for performance matters
            // since cache size is not configurable by consumer. It allows 
            // to have an optimal loop without multiple conditions checks 
            // and vector reallocation.

            static constexpr int k0 = 541;
            static std::vector<int> array;

            // Sieve of the first 100 primes (last prime is k0).
            // This is a basic straight-forward implementation of
            // the Sieve of Eratosthenes since our cache is very
            // small. Better optimized approches (i.e Wheel) should
            // be considered if our cache size increases.
            static inline void init() {
                int a[k0] = { 0 };
                for (int i = 2; i <= k0; i++) {
                    for (int j = i * i; j <= k0; j += i) {
                        a[j - 1] = 1;
                    }
                }
                array.reserve(100);
                for (int i = 2; i <= k0; i++) {
                    if (a[i - 1] == 0) {
                        array.push_back(i);
                    }
                }
            }

            static inline bool nearest(int i, int m, int dir) {
                if (dir == 1) {
                    return array[m > 0 ? m - 1 : 0] < i && array[m] > i;
                }
                return array[m] < i && array[m < array.size() ? m + 1 : array.size()] > i;
            }

            // If match == true dir is omitted, otherwise return first 
            // element < || > value. Return 0 if match == true and value 
            // is not found.
            static inline int bsearch(int i, int dir, bool match) {
                auto const size = array.size();
                if (size == 0) {
                    init();
                }
                int l(0), r(size - 1), m(0);
                while (r >= l) {
                    m = (l + r) >> 1;
                    if (array[m] == i || (!match && nearest(i, m, dir))) {
                        return array[m];
                    }
                    else if (array[m] < i) {
                        l = m + 1;
                    }
                    else {
                        r = m - 1;
                    }
                }
                return 0;
            }

            // Helper function to check if value is prime where value is odd.
            static inline BOOL is_impl(int i) {

                // Checks modulo 3 to reduce iterations in the next step.
                // Note: value == 3 returns a false negative but this should 
                // never happen since value < k0 is tested in cache.
                if (i % 3 == 0) {
                    return false;
                }
                for (int j = 5; j * j <= i; j += 6) {
                    if (i % j == 0 || i % (j + 2) == 0) {
                        return false;
                    }
                }
                return true;
            }

            // Moves value to an odd and checks for next odds in a loop
            // until a prime is found. For obvious performance matters 
            // this function should not be called when value < k0 and 
            // therefore contained in cache.
            static inline int search(int i, int dir) {
                i += (i % 2) + dir;
                for (int j = i;; j += dir * 2) {
                    if (is_impl(j)) {
                        return j;
                    }
                }
            }

            int find(int i, int dir) {
                if (i <= 2) {
                    return 2;
                }
                if (i > k0) {
                    return search(i, dir);
                }
                return bsearch(i, dir, false);
            }

	    } // namespace detail

    } // namespace prime

} // namespace circus