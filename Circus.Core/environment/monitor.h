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
// Physical displays information.


#pragma once

#include <windef.h>
#include "detail/monitor-detail.h"

namespace circus {

	namespace environment {

		namespace monitor {

			BOOL Size(int f, double& w, double& h) {
				std::pair<double, double> pair;
				BOOL r = monitor::detail::GetSize(f, &pair);
				if (r) {
					w = std::get<0>(pair);
					h = std::get<1>(pair);
				}
				return r;
			}

		} // namespace monitor

	} // namespace environment

} // namespace circus