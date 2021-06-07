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
// A component to create and manage memory allocated objects.
//
// The purpose was to have a convenient way of managing object instances
// that are either singletons or, as described below, shared.
//
// The component uses a thread-static instance which ensures uniqueness
// for each threads.
//
// It provides full lazy instanciation and thread-safety.
//
// Instances are stored in an internal map of strings which corresponds to 
// the instances identifiers (like variables names).
//
// Singleton are created when initally invoked. The component uses the
// parameterless constructor that can either be public, internal or
// private. Since they are unique for a specific type, they are registered
// using their assembly qualified name.
//
// Shared instances are intended for cases where an object must be accessed
// by multiple classes that are not related to each other and therefore
// have no reference to the object.
//
// These objects must be allocated using the Make() methods. The component
// uses either the parameterless constructor or the constructor that best
// matches the specified parameters. Constructor must be public if args is
// supplied.
//
// They can be released using the Release() method. If the object is
// IDisposable, Dispose() is called.
//
// Note that package elements (i.e. DockViewSelectionInfo) use Allocator.
// To avoid collisions with consumer variables names, instances use a guid
// as identifier.
//
// Assignment returns a boolean which allows to allocate objects in nested
// statements. It alwyas returns true. It is mainly used to assign local
// fields or output parameters such as:
//
// public bool Foo(object obj, out object result) {
//      return SomeMethod(obj, out result) || !Allocator.Assign(null, out result);
// }
//
// In this example, Foo assigns result and returns true if SomeMethod 
// succeeded, otherwise it assigns null to result and returns false.
//
// Performance has a little overhead compared to class defined objects 
// since instances are retrieved from a container.


#pragma warning disable IDE0002

using System;
using Circus.Collections;
namespace Circus.Runtime {
    /// <summary>Provides a component to create and manage memory allocated objects.</summary>
    public sealed class Allocator {
        private readonly Map<string, object> array;
        [ThreadStatic]
        private static Lazy<Allocator> current;
        private static Allocator Current {
            get {
                if (Assert.Null(Allocator.current) || !Allocator.current.IsValueCreated) {
                    Allocator.current = new Lazy<Allocator>(() => new Allocator());
                }
                return Allocator.current.Value;
            }
        }
        private Allocator() {
            this.array = new Map<string, object>(11, 11);
        }
        private T AddOrGet<T>() {
            return Allocator.Assign(typeof(T), out Type type) & Allocator.Assign(type.AssemblyQualifiedName, out string name) & (!this.array.Get(name, out object value) && (!Allocator.CreateInstance(type, ref value, true, null) || !this.array.Add(name, value))) ? default : (T)value;
        }
        /// <summary>Assigns the provided value to the out T object. This method always returns true.</summary>
        public static bool Assign<T>(object value, out T result) {
            result = (T)value;
            return true;
        }
        private static bool CreateInstance(Type type, ref object value, bool flag, params object[] args) {
            try {
                return Allocator.Assign(flag ? Activator.CreateInstance(type, true) : Activator.CreateInstance(type, Assert.Null(args) ? new object[] { } : args), out value);
            }
            catch {
                return false;
            }
        }
        private T Find<T>(string name) {
            return this.array.Get(name, out object value) ? (T)value : default;
        }
        /// <summary>Returns the shared instance of the specified T type. T must be a class.</summary>
        public static T Get<T>(string name) where T : class {
            return Allocator.Current.Find<T>(name);
        }
        private bool Insert<T>(string name, params object[] args) where T : class {
            if (Allocator.Assign(typeof(T), out Type type) & this.array.Get(name, out object value)) {
                return false;
            }
            return Allocator.CreateInstance(type, ref value, Assert.Null(args), args) && this.array.Add(name, value);
        }
        /// <summary>Creates a new shared instance of the specified T type using the public parameterless constructor. T must be a class. Returns true if creation succeeded.</summary>
        public static bool Make<T>(string name) where T : class {
            return Allocator.Make<T>(name, null);
        }
        /// <summary>Creates a new shared instance of the specified T type using the specified parameters. T must be a class. Returns true if creation succeeded.</summary>
        public static bool Make<T>(string name, params object[] args) where T : class {
            return Allocator.Current.Insert<T>(name, args);
        }
        /// <summary>Releases the shared instance with the specified name. Returns true if releasing succeeded.</summary>
        public static bool Release(string name) {
            return Allocator.Current.Remove(name);
        }
        private bool Remove(string name) {
            if (Allocator.Assign(this.array.Get(name, out object value) && this.array.Remove(name), out bool num) && num && Assert.Is(value, out IDisposable disposable)) {
                disposable.Dispose();
            }
            return num;
        }
        /// <summary>Returns a singleton instance of the specified T type. T must be a class with a parameterless constructor either public, internal or private.</summary>
        public static T Singleton<T>() where T : class {
            return Allocator.Current.AddOrGet<T>();
        }
    }
}
