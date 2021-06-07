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
// Some functions to check or find primes.
//
// The strategy is to generate a cache of primes that are fast and easy
// to use for most cases, and then have some functions to calculate the
// previous or next prime when a larger set is required.
//
// Cache is populated by a Sieve at first run containing the first 100
// primes (ends at 541). This allows to perform a binary search on static
// data when value is within range, otherwise fall back to the calculation
// functions.


#pragma once

#include <windef.h>
#include "detail/prime-detail.h"

namespace circus {

    namespace prime {

        BOOL is(int i) {
            if (i < 2 || (i > 2 && i % 2 == 0)) {
                return false;
            }
            if (i < prime::detail::k0) {

                // Use any value for dir since it will be omitted 
                // in BinarySearch due to match == true.
                return prime::detail::bsearch(i, 0, true) != 0;
            }
            return prime::detail::is_impl(i);
        }

        // Returns the next prime based on dir, or value if it's a prime. 
        // dir == 1 for next or -1 for previous, otherwise returns 0.
        int next(int i, int dir) {
            if (abs(dir) != 1) {
                return 0;
            }
            if (prime::is(i)) {
                return i;
            }
            return prime::detail::find(i, dir);
        }

    } // namespace prime

} // namespace circus
