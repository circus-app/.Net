#pragma warning disable IDE0002

using System;
using System.Reflection;
namespace Circus.Runtime {
    public static class Traits {
        public static bool GetAttribute<T>(object obj, out T attribute) {
            return Allocator.Assign(obj.GetType().GetCustomAttribute(typeof(T), true), out attribute) && Assert.NotNull(attribute);
        }
        public static bool GetPropertyInfo(object obj, string name, out PropertyInfo info) {
            return Allocator.Assign(obj.GetType(), out Type type) & Allocator.Assign(type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), out info);
        }
        public static bool GetPropertyValue<T>(object obj, string name, out T value) {
            return (Traits.GetPropertyInfo(obj, name, out PropertyInfo info) && Allocator.Assign((T)info.GetValue(obj), out value)) || !Allocator.Assign(default, out value);
        }
    }
}
