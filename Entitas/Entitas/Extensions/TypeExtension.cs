using System;
using System.Linq;

namespace Entitas {

    public static class TypeExtension {

        public static bool ImplementsInterface<T>(this Type type) {
            if (!type.IsInterface && type.GetInterfaces().Contains(typeof(T))) {
                return true;
            }

            return false;
        }
    }
}