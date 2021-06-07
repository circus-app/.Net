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
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the 
// "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
// Partial implementation of Google's Farmhash library for string hashing. 
// It uses the Hash64() function to return an unsigned 64-bit integer.
//
// The library has been designed for fast and efficient hash distribution,
// and low collisions. It uses different functions based on string length
// and is optimized for hash tables or hash maps.
//
// Another good reason to use an external lib is that hashing does not rely
// on instance, therefore the same string will always produce the same hash
// (which is not guaranteed in .net framework). This could be interesting 
// in cases where we would need to use cached values.
//
// 32-bit hash functions have been skipped since they're less performant 
// and only relevant for old architectures.
//
// Farmhash in a non-crytographic algorythm, therefore it is not suited for
// encrypting passwords and/or secured data.


#pragma once

#include <stdint.h>
#include "detail/farmhash-detail.h"

namespace circus {

    namespace farmhash {

        uint64_t hash64(const char* str, size_t size) {
            return farmhash::farmx::Hash64(str, size);
        }

    } // namespace farmhash

} // namespace circus

