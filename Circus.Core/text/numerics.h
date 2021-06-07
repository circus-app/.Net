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
// String to numeric types.


#pragma once

#include <windef.h>
#include "basic_string.h"

namespace circus {

	namespace numerics {

		// Determines if str is a number, s states if it's signed and d if
		// it's a decimal. The function does not support typed values
		// (e.g. 1U) nor E-notation (e.g. 1e-9).
		BOOL is(const circus::text::basic_string& str, BOOL& s, BOOL& d) {
			auto const len = str.length();

			// Check if it's signed.
			auto const minus = str[0] == '-';

			// Return false if str is '-', '-.' or '.'
			if ((minus && (len == 1 || (len > 1 && str[1] == '.'))) || str[0] == '.') {
				return false;
			}

			// Find any non-numeric char.
			BOOL r = str.first_not_of("0123456789.", minus ? 1 : 0) == -1;

			// Determine if it's decimal and ensure that it does not contain
			// more than 1 separator.
			if (r) {
				auto const i = str.first('.');
				if (i != -1) {
					if (str.last('.') == i) {
						d = true;
					}
					else {
						// More than 1 separator is found.
						r = false;
					}
				}
			}

			// Output signed flag.
			if (r && minus) {
				s = minus;
			}
			return r;
		}
	
	} // namespace numerics

} // namespace circus


