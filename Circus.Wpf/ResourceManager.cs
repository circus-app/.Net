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
// A manager of embedded resources.
//
// Loads at runtime resources located in the executing assembly or one of
// its dependencies. It is not designed to manage external resources such
// as files, libraries, etc.
//
// The component provides a set of locations from which resources are
// loaded in the application domain using the Get(alias, name) method.
//
// Each location is associated to an alias, which corresponds to a map key.
// Aliases must be non-null nor whitespaces and unique.
//
// Resource locations are registered using the Add(domain, locations) 
// method. The domain corresponds the namespace where resources are 
// located. Locations parameter is an array that allows registering multiple
// locations for a specific namespace.
//
// Since resources are most probably static xaml files, a good practice is
// to register locations in the App.xaml.cs file:
//
// protected override void OnStartup(StartupEventArgs e) {
//      base.OnStartup(e);
//      ResourceManager.Add("SomeNamespace", 
//          new ResourceLocation("Images", "MyResources/Images"));
// }
//
// Get(alias, name) method outputs an object instance, created from the
// specified file at the corresponding location. If no file extension is
// provided, it is assumed being .xaml.
//
// If the resource is IResource, its location full path is set to its 
// IResource.Id property. This allows the Get(obj) or Get(IResource) methods
// to create an object instance using its original path.
//
// This is done for objects that need to create copies of themselves. Since
// .Net does not provide a convenient way of performing such a task, we 
// need a cache mecanism of created objects to easily recreate copies when 
// required.

// A good example is the icon property of a toolbar button. Because this 
// property is bound to the button itself but also to the descriptor view, 
// we need two instances of the same icon. Otherwise, we would need to 
// specify the binding twice (in the content property and in the icon 
// property) which is not very user friendly.
//
// The class is not thread-safe nor serializable.


#pragma warning disable IDE0002

using System;
using System.IO;
using System.Windows;
using Circus.Collections;
using Circus.Runtime;
namespace Circus.Wpf {
    /// <summary>Provides a manager of embedded resources.</summary>
    public sealed class ResourceManager {
        private readonly Map<string, string> array;
        private static ResourceManager Current => Allocator.Singleton<ResourceManager>();
        private ResourceManager() {
            this.array = new Map<string, string>();
        }
        /// <summary>Adds the provided resource locations for the specified domain to the resource manager.</summary>
        public static void Add(string domain, params ResourceLocation[] locations) {
            ResourceManager.Current.Insert(domain, locations);
        }
        /// <summary>Determines if the resource manager contains the specified alias.</summary>
        public static bool Contains(string alias) {
            return ResourceManager.Current.Find(alias);
        }
        internal static bool Contains(string alias, string name, out string value) {
            return ResourceManager.Current.Find(alias, name, out value);
        }
        private bool Delete(string alias) {
            return this.array.Remove(alias);
        }
        private bool Find(string alias) {
            return this.array.Contains(alias);
        }
        private bool Find(string id, out object value) {
            value = this.LoadComponent(id);
            return Assert.NotNull(value);
        }
        private bool Find(object source, out object value) {
            return (Assert.Is(source, out IResource resource) && this.Find(resource.Id, out value)) || !Allocator.Assign(null, out value);
        }
        private bool Find(string alias, string name, out object value) {
            value = this.Find(alias, name, out string path) ? this.LoadComponent(path) : null;
            return Assert.NotNull(value);
        }
        private bool Find(string alias, string name, out string value) {
            value = this.array.Get(alias, out string path) ? string.Format("{0}/{1}", path, ResourceManager.GetFileName(name)) : null;
            return Assert.NotNull(value);
        }
        /// <summary>Outputs an instance of the specified resource id. Returns true if resource exists.</summary>
        public static bool Get(string id, out object value) {
            return ResourceManager.Current.Find(id, out value);
        }
        /// <summary>Outputs a copy of the specified IResource source. Returns true if copy succeeded.</summary>
        public static bool Get(IResource source, out object value) {
            return ResourceManager.Current.Find(source.Id, out value);
        }
        /// <summary>Outputs a copy of the specified source object. Returns true if object is IResource and copy succeeded.</summary>
        public static bool Get(object source, out object value) {
            return ResourceManager.Current.Find(source, out value);
        }
        /// <summary>Outputs an instance of the specified named resource for the provided alias. Returns true if the resource exists.</summary>
        public static bool Get(string alias, string name, out object value) {
            return ResourceManager.Current.Find(alias, name, out value);
        }
        private static string GetFileName(string name) {
            return Assert.Null(Path.GetExtension(name)) ? string.Format("{0}.xaml", name) : name;
        }
        private void Insert(string domain, params ResourceLocation[] locations) {
            foreach (ResourceLocation location in locations) {
                this.array.AddOrUpdate(location.Alias, string.Format("/{0};component/{1}", domain, location.Path));
            }
        }
        private object LoadComponent(string value) {
            try {
                object obj = Application.LoadComponent(new Uri(value, UriKind.Relative));
                if (Assert.Is(obj, out IResource resource)) {
                    resource.Id = value;
                }
                return obj;
            }
            catch {
                return null;
            }
        }
        /// <summary>Removes the specified alias from the resource manager. Returns true if removal succeeded.</summary>
        public static bool Remove(string alias) {
            return ResourceManager.Current.Delete(alias);
        }
    }
}
