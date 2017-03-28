using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Entitas {

    public static class TypeUtils {

        public static Type[] GetAllTypes() {
            var types = new List<Type>();
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                try {
                    types.AddRange(assembly.GetTypes());
                } catch(ReflectionTypeLoadException ex) {
                    types.AddRange(ex.Types.Where(type => type != null));
                }
            }

            return types.ToArray();
        }

        public static Type[] GetNonAbstractTypes<T>() {
            return GetAllTypes()
                    .Where(type => !type.IsAbstract)
                    .Where(type => type.ImplementsInterface<T>())
                    .ToArray();
        }

        public static T[] GetInstancesOf<T>() {
            return GetNonAbstractTypes<T>()
                    .Select(type => (T)Activator.CreateInstance(type))
                    .ToArray();
        }
    }
}
