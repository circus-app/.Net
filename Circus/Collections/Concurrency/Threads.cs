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


using System.Threading;
namespace Circus.Collections.Concurrency {
    internal sealed class Threads<T> {
        private readonly object[] array;
        internal T[] Entries;        
        private Threads(int concurrency) {
            array = new object[concurrency > 1024 ? 1024 : concurrency];
        }
        internal Threads(int concurrency, int size) : this(concurrency) {
            this.Entries = new T[size];
            this.Initialize();
        }
        internal bool Enter(int index, out object obj) {
            bool f = false;
            obj = array[index % array.Length];
            Monitor.Enter(obj, ref f);
            return f;
        }
        internal void Exit(object obj) {
            Monitor.Exit(obj);
        }
        internal int Freeze() {
            int num = 0;
            for (int i = 0; i < array.Length; i++) {
                bool f = false;
                Monitor.Enter(array[i], ref f);
                num += f ? 1 : 0;
            }
            return num;
        }
        private void Initialize() {
            for (int i = 0; i < array.Length; i++) {
                array[i] = new object();
            }
        }
        internal void Unfreeze(int count) {
            for (int i = 0; i < count; i++) {
                Monitor.Exit(array[i]);
            }
        }
    }
}
