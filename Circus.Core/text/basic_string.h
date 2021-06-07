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
// A basic string wrapper around char*.
//
// The class was initially designed to provide a standard c++ char* buffer
// since CLR char size is 2 and c++ is 1. This allows to provide fixed 
// char* to PInvokes and therefore skip the marshalling layer that has a
// massive performance cost.
//
// It does not perform zero size checks to avoid useless malloc.
// This must be checked in calling functions prior to creating the object.
//
// Implementation is highly based on Facebook's folly/FBstring, mainly for
// the finding part. However, this does not implement all member functions
// required by the standard and therefore cannot be considered as a 
// replacement to a std::string type.
//
// Finding uses a Boyer-Moore like implementation by comparing last 
// characters first, and padding the search to a skip index when needed. 
// Performance is increased by 20-30% on medium to large strings than the 
// naive search in CLR. It is slightly equal on small strings (<= 20 chars).
//
// Hashing is provided by a partial implementation of Google's Farmhash. 
// See hash/farmhash.h for details.


#pragma once

#include <algorithm>
#include <cassert>
#include <string>

#include "../hash/farmhash.h"

namespace circus {

	namespace text {

		class basic_string {
		public:

			// Types
			typedef std::char_traits<char> traits_type;
			typedef typename traits_type::char_type value_type;
			typedef std::allocator<char> allocator_type;
			typedef typename std::allocator_traits<allocator_type>::size_type size_type;
			typedef typename std::allocator_traits<allocator_type>::value_type const& const_reference;

			typedef const char* const_iterator;
			static constexpr size_type npos = size_type(-1);

		public:
			basic_string() = delete;

			basic_string(const basic_string& string) = delete;

			basic_string(const value_type* s, size_type n) noexcept : size_(n) {
				init(s);
			}

			~basic_string() noexcept {
				free(data_);
			}

			basic_string& operator=(const basic_string& str) = delete;

			const_reference operator[](size_type pos) const {
				return *(begin() + pos);
			}

			const_iterator begin() const {
				return data();
			}

			int compare(const basic_string& str) const {
				const size_type n1(size()), n2(str.size());
				const int r = traits_type::compare(data(), str.data(), std::min(n1, n2));
				return r != 0 ? r : n1 > n2 ? 1 : n1 < n2 ? -1 : 0;
			}

			value_type* data() const {
				return data_;
			}

			const_iterator end() const {
				return data() + size();
			}

			size_type first(const basic_string& str, size_type pos = 0) const {
				return first(str.data(), pos, str.size());
			}

			size_type first(const value_type* s, size_type pos = 0) const {
				return first(s, pos, traitsLength(s));
			}

			size_type first(value_type c, size_type pos = 0) const {
				return first(&c, pos, 1);
			}

			size_type first_not_of(const basic_string& str, size_type pos = 0) const {
				return first_not_of(str.data(), pos, str.size());
			}

			size_type first_not_of(const value_type* s, size_type pos = 0) const {
				return first_not_of(s, pos, traitsLength(s));
			}

			size_type first_not_of(value_type c, size_type pos = 0) const {
				return first_not_of(&c, pos, 1);
			}

			uint64_t hash() {
				return farmhash::hash64(this->data(), this->size());
			}

			size_type last(const basic_string& str, size_type pos = npos) const {
				return last(str.data(), pos, str.size());
			}

			size_type last(const value_type* s, size_type pos = npos) const {
				return last(s, pos, traitsLength(s));
			}

			size_type last(value_type c, size_type pos = npos) const {
				return last(&c, pos, 1);
			}

			size_type last_not_of(const basic_string& str, size_type pos = npos) const {
				return last_not_of(str.data(), pos, str.size());
			}

			size_type last_not_of(const value_type* s, size_type pos = npos) const {
				return last_not_of(s, pos, traitsLength(s));
			}

			size_type last_not_of(value_type c, size_type pos = npos) const {
				return last_not_of(&c, pos, 1);
			}

			size_type length() const {
				return size();
			}

			size_type size() const {
				return size_;
			}

			static size_type traitsLength(const value_type* s);

		private:
			inline size_type find(const value_type* s, const size_type pos, const size_type n) const;
			inline size_type first(const value_type* s, size_type pos, size_type n) const;
			inline size_type first_not_of(const value_type* s, size_type pos, size_type n) const;
			inline void init(const value_type* s);
			inline size_type last(const value_type* s, size_type pos, size_type n) const;
			inline size_type last_not_of(const value_type* s, size_type pos, size_type n) const;

		private:
			value_type* data_;
			const size_type size_;
		};

		inline typename basic_string::size_type
			basic_string::find(const value_type* s,
				const size_type pos,
				const size_type n) const {
			auto const size = this->size();

			// n + pos can overflow (eg pos == npos), guard against that by checking
			// that n + pos does not wrap around.
			if (n + pos > size || n + pos < pos) {
				return npos;
			}

			if (n == 0) {
				return pos;
			}
			// Don't use std::search, use a Boyer-Moore-like trick by comparing
			// the last characters first
			auto const stack = this->data();
			auto const f = n - 1;
			auto const last = s[f];

			// Boyer-Moore skip value for the last char in s. Zero is not a valid value; 
			// skip will be computed the first time it's needed.
			size_type skip = 0;

			const value_type* i = stack + pos;
			auto end = stack + size - f;

			while (i < end) {
				// Boyer-Moore: match the last element in s.
				while (i[f] != last) {
					if (++i == end) {
						// Not found.
						return npos;
					}
				}
				// Last character matches.
				for (size_t j = 0;;) {
					assert(j < n);
					if (i[j] != s[j]) {
						// Not found, we can skip and compute the value lazily.
						if (skip == 0) {
							skip = 1;
							while (skip <= f && s[f - skip] != last) {
								++skip;
							}
						}
						i += skip;
						break;
					}
					// Return if done searching.
					if (++j == n) {
						return i - stack;
					}
				}
			}
			return npos;
		}

		inline typename basic_string::size_type
			basic_string::first(
				const value_type* s,
				size_type pos,
				size_type n) const {
			if (pos > length()) {
				return npos;
			}
			const_iterator i(begin() + pos), e(end());
			for (; i != e; ++i) {
				if (traits_type::find(s, n, *i) != nullptr) {
					return i - begin();
				}
			}
			return npos;
		}

		inline typename basic_string::size_type
			basic_string::first_not_of(
				const value_type* s,
				size_type pos,
				size_type n) const {
			if (pos < length()) {
				const_iterator i(begin() + pos), e(end());
				for (; i != e; ++i) {
					if (traits_type::find(s, n, *i) == nullptr) {
						return i - begin();
					}
				}
			}
			return npos;
		}

		inline void basic_string::init(const value_type* s) {

			// Copy s buffer padding left 1 byte since s char size == 2 
			// and this char size == 1.
			auto const size = this->size();
			data_ = (value_type*)malloc(size + 1);
			assert(data_ != NULL);
			for (int i = 0; i < size; ++i) {
				data_[i] = s[i << 1];
			}
			// Append a null terminator for comformity since s is not marshalled and
			// CLR does not specify any null-terminated requirement.
			data_[size] = '\0';
		}

		inline typename basic_string::size_type
			basic_string::last(
				const value_type* s,
				size_type pos,
				size_type n) const {
			if (n > 0) {
				pos = std::min(pos, length() - 1);
				const_iterator i(begin() + pos);
				for (;; --i) {
					if (traits_type::find(s, n, *i) != nullptr) {
						return i - begin();
					}
					if (i == begin()) {
						break;
					}
				}
			}
			return npos;
		}

		inline typename basic_string::size_type
			basic_string::last_not_of(
				const value_type* s,
				size_type pos,
				size_type n) const {
			pos = std::min(pos, length() - 1);
			const_iterator i(begin() + pos);
			for (;; --i) {
				if (traits_type::find(s, n, *i) == nullptr) {
					return i - begin();
				}
				if (i == begin()) {
					break;
				}
			}
			return npos;
		}

		inline typename basic_string::size_type
			basic_string::traitsLength(const value_type* s) {
			return s ? traits_type::length(s) : 0;
		}

	} // namespace text

} // namespace circus
