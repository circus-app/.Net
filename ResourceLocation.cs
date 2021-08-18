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
// Location info of a resource for the resource manager.
//
// Alias is the resource key for the specified path. Path is the relative 
// path to the resource using the file uri scheme.

namespace Circus.Wpf {
    /// <summary>Provides location info of a resource for the resource manager.</summary>
    public sealed class ResourceLocation {
        /// <summary>Returns the alias of the resource.</summary>
        public string Alias { get; private set; }
        /// <summary>Returns the relative path of the resource.</summary>
        public string Path { get; private set; }
        private ResourceLocation() { 
        }
        /// <summary>Constructs a ResourceLocation with the specified alias and path.</summary>
        public ResourceLocation(string alias, string path) {
            this.Alias = alias;
            this.Path = path;
        }
    }
}
