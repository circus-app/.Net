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


#pragma warning disable IDE0002

using System;
using Circus.Collections;
using Circus.Runtime;

namespace Circus.Wpf.Data {
    internal sealed class DataSources {
        private readonly Map<string, DataSource> array;
        private static DataSources Current => Allocator.Singleton<DataSources>();
        private DataSources() {
            this.array = new Map<string, DataSource>();
        }
        private T Find<T>() where T : DataSource {
            return (T)this.Find(typeof(T));
        }
        private DataSource Find(Type type) {
            if (!this.array.Contains(type.Name)) {
                this.array.Add(type.Name, (DataSource)Activator.CreateInstance(type, true));
            }
            return this.array.Get(type.Name, out DataSource data) ? data : null;
        }
        internal static T Get<T>() where T : DataSource {
            return DataSources.Current.Find<T>();
        }
        internal static DataSource Get(Type type) {
            return DataSources.Current.Find(type);
        }
    }
}
