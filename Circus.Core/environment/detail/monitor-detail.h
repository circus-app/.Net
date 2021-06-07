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

#include <iostream>
#include <windows.h>

namespace circus {

	namespace environment {
		
		namespace monitor {

			namespace detail {

				// Returns the monitor handle from the cursor location.
				static inline HMONITOR MonitorFromCursor() {
					POINT p;
					return GetCursorPos(&p) ? MonitorFromPoint(p, MONITOR_DEFAULTTONEAREST) : NULL;
				}

				// Sets the value of the specified HMONITOR pointer. Monitor handle
				// is determined by the active window if it's not null and not hidden, 
				// otherwise provides the monitor that contains the mouse pointer.
				// Returns true if the handle is not null.
				static inline BOOL GetMonitor(HMONITOR* handle) {
					auto hwnd = GetActiveWindow();
					*handle = hwnd != NULL && IsWindowVisible(hwnd) ? MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST) : MonitorFromCursor();
					return handle != NULL;
				}

				// Sets the value of the specified MONITORINFOEX. Returns true
				// if a valid monitor handle is found and the GetMonitorInfoW
				// function succeeded.
				static inline BOOL GetInfo(MONITORINFOEX* info) {
					HMONITOR handle;
					return GetMonitor(&handle) && GetMonitorInfoW(handle, info);
				}

				// Provides a pair of double containing the logical size of the monitor 
				// that hosts the active window. f == 0 -> monitor, otherwise work area.
				// Returns true if the function succeeded.
				BOOL GetSize(int f, std::pair<double, double>* pair) {
					MONITORINFOEX info;
					info.cbSize = sizeof(MONITORINFOEX);
					BOOL i = GetInfo(&info);
					if (i) {
						RECT r = f == 0 ? info.rcMonitor : info.rcWork;
						*pair = std::make_pair((double)r.right - (double)r.left, (double)r.bottom - (double)r.top);
					}
					return i;
				}

			} // namespace detail

		} // namespace monitor
	
	} // namespace environment
	
} // namespace circus