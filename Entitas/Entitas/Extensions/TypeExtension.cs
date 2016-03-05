using System;
using System.Linq;

namespace Entitas {

    public static class TypeExtension {

        /// Determines whether the type implements the specified interface and is not an interface itself.
        public static bool ImplementsInterface<T>(this Type type) {
            if (!type.IsInterface && type.GetInterfaces().Contains(typeof(T))) {
                return true;
            }

            return false;
        }
    }
}