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
// A collection of reusable IResources associated to an object.
//
// The purpose was to have a component to optimize memory allocation of
// reusable IResource objects.
//
// Let's consider a floating window. Its header shows an icon that is 
// the icon of the selected document in the document tray. Since the icon
// property is attached to the document object, we need to create a copy 
// for the window header. This is provided by the Get(obj) method of the
// ResourceManager using the IResource interface.
//
// On document tray selection change, the window icon is updated with the 
// new selected document icon. A straightforward implementation would be 
// to request the ResourceManager for a copy of the new icon and assign it 
// to the window icon but this would create an object in memory on each
// selection change.
// 
// Furthermore, document icons may be identical which would end up 
// allocating the same object many times.
//
// The ResourceCache is here to avoid the situation where multiple 
// identical resources are created for a specific object.
// 
// If we go back to the above example, on document added, the floating 
// window registers the document icon to the resource cache. On document 
// tray selection change, the floating window requests the resource cache 
// for an item which IResource.Id equals the newly selected document icon 
// (as IResource.Id). If found, it returns the cached IResource object.
//
// The component stores the items in a weak map of objects (aka sources), 
// each mapped to a map of IResource ids and their related IResource 
// object.
//
// Even though sources are weak references, it is recommended to explicitly
// remove the objects from the cache when they are no longer used and/or
// destroyed to avoid dead references (see Circus/WeakMap for more 
// details on dead references cleaning).


#pragma warning disable IDE0002

using Circus.Collections;
using Circus.Collections.Conditional;
using Circus.Runtime;
namespace Circus.Wpf {
    /// <summary>Provides a collection of reusable IResources associated to an object.</summary>
    public sealed class ResourceCache {
        private readonly WeakMap<object, Map<string, object>> array;
        private static ResourceCache Current => Allocator.Singleton<ResourceCache>();
        private ResourceCache() {
            this.array = new WeakMap<object, Map<string, object>>();
        }
        private bool Delete(object source) {
            return this.array.Remove(source);
        }
        /// <summary>Outputs the cached value mapped to the specified source. Returns true if value is not null and a cached element is found or created.</summary>
        public static bool Get(object source, object value, out object result) {
            if (Assert.Null(value) && Allocator.Assign(null, out result)) {
                return false;
            }
            return ResourceCache.Current.Find(source, value, out result) || ResourceCache.Current.Insert(source, value, out result);
        }
        internal static bool Get(object source, string alias, string name, out object value) {
            return (ResourceManager.Contains(alias, name, out string id) && (ResourceCache.Current.Find(source, id, out value) || ResourceCache.Current.Insert(source, id, out value))) || !Allocator.Assign(null, out value);
        }
        private bool Insert(object source, object value, out object obj) {
            return (Assert.As(value, out IResource resource) && this.Insert(source, resource.Id, out obj)) || !Allocator.Assign(null, out obj);
        }
        private bool Insert(object source, string id, out object obj) {
            Map<string, object> array = new Map<string, object>();
            return (this.array.Add(source, array) && ResourceManager.Get(id, out obj) && array.Add(id, obj)) || !Allocator.Assign(null, out obj);
        }
        private bool Find(object source, object value, out object result) {
            return this.Find(source, ((IResource)value).Id, out result);
        }
        private bool Find(object source, string id, out object result) {
            result = this.array.Get(source, out Map<string, object> array) && array.Get(id, out object obj) ? obj : null;
            return Assert.NotNull(result);
        }
        /// <summary>Removes the specified source from the resource cache. Returns true if removal succeeded.</summary>
        public static bool Remove(object source) {
            return ResourceCache.Current.Delete(source);
        }
    }
}
