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
// Provides the alias that is registered in the ResourceManager for
// package resources. It is a guid to avoid collisions with consumer
// aliases. For internal use only.


namespace Circus.Wpf.Images {
    internal sealed class Image {
        /// <summary>Returns the ResourceManager alias for package resoures.</summary>
        public const string Alias = "025AC7D595294147B20E2E1860475A67";
        private Image() { 
        }
    }
}
