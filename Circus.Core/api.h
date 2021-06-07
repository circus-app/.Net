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
// Circus core library for Windows .net runtime.
//
// The library provides optimized functions to support .net runtime 
// in features either not available or with poor performances in 
// managed code.
//
// It is targeted for Windows 64-bit environment ONLY.
//
// Circus namespace is the root namespace that contains all functions
// available for PInvoke.
//
// Except for api files, the library is headers only. Internal functions 
// are in detail namespace since they are not intended to be used outside 
// their parent scope.
//
// We use Win32 BOOL because VS team decided to marshal booleans as 4 bytes 
// integers and standard c++ bool is 1 byte. This is to avoid confusion or 
// having to specify an unmanaged return type in PInvokes.


#pragma once

// Remove min/max windef macros. Use std instead.
#ifndef NOMINMAX
#define NOMINMAX
#endif

// Target architecture.
#ifndef _AMD64_
#define _AMD64_ 1
#endif

#ifndef EXPORT_TO_API
#define EXPORT_TO_API __declspec(dllexport)
#endif

#include <stdint.h>
#include <windef.h>

#include "environment/monitor.h"
#include "hash/prime.h"
#include "text/basic_string.h"
#include "text/numerics.h"

namespace circus {

	// String functions.
	extern "C" EXPORT_TO_API int Contains(const char* str, int n, const char* str1, int n1);
	extern "C" EXPORT_TO_API BOOL Equals(const char* str, int n, const char* str1, int n1);
	extern "C" EXPORT_TO_API int FirstNotOf(const char* str, int n, const char* str1, int n1);
	extern "C" EXPORT_TO_API BOOL Hash(const char* str, int n, uint64_t& hash);
	extern "C" EXPORT_TO_API BOOL IsNumeric(const char* str, int n, BOOL& s, BOOL& d);
	extern "C" EXPORT_TO_API int Last(const char* str, int n, const char* str1, int n1);
	extern "C" EXPORT_TO_API int LastNotOf(const char* str, int n, const char* str1, int n1);

	// Numeric functions.
	extern "C" EXPORT_TO_API BOOL IsPrime(int value);
	extern "C" EXPORT_TO_API int NextPrime(int value);
	extern "C" EXPORT_TO_API int PreviousPrime(int value);

	// Environment functions.
	extern "C" EXPORT_TO_API BOOL MonitorSize(int flag, double& width, double& height);

} // namespace circus