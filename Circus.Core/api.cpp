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


#include "api.h"

namespace circus {

	// String functions.
	int Contains(const char* str, int n, const char* str1, int n1) {
		return n == 0 || n1 == 0 ? -1 : (int)circus::text::basic_string(str, n).first(circus::text::basic_string(str1, n1));
	}

	BOOL Equals(const char* str, int n, const char* str1, int n1) {
		return n != n1 ? false : circus::text::basic_string(str, n).compare(circus::text::basic_string(str1, n1)) == 0;
	}

	int FirstNotOf(const char* str, int n, const char* str1, int n1) {
		return (int)circus::text::basic_string(str, n).first_not_of(circus::text::basic_string(str1, n1));
	}

	BOOL Hash(const char* str, int n, uint64_t& hash) {
		if (n == 0) {
			return false;
		}
		hash = circus::text::basic_string(str, n).hash();
		return true;
	}

	BOOL IsNumeric(const char* str, int n, BOOL& s, BOOL& d) {
		return n == 0 ? false : numerics::is(circus::text::basic_string(str, n), s, d);
	}

	int Last(const char* str, int n, const char* str1, int n1) {
		return n == 0 || n1 == 0 ? -1 : (int)circus::text::basic_string(str, n).last(circus::text::basic_string(str1, n1));
	}

	int LastNotOf(const char* str, int n, const char* str1, int n1) {
		return (int)circus::text::basic_string(str, n).last_not_of(circus::text::basic_string(str1, n1));
	}

	// Numeric functions.
	BOOL IsPrime(int i) {
		return prime::is(i);
	}

	int NextPrime(int i) {
		return prime::next(i, 1);
	}

	int PreviousPrime(int i) {
		return prime::next(i, -1);
	}

	// Environment functions.
	BOOL MonitorSize(int f, double& w, double& h) {
		return environment::monitor::Size(f, w, h);
	}

} // namespace circus