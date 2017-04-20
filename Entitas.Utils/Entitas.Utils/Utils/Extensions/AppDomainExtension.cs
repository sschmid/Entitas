using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Entitas.Utils {

    public static class AppDomainExtension {

        public static Type[] GetAllTypes(this AppDomain appDomain) {
            var types = new List<Type>();
            foreach (var assembly in appDomain.GetAssemblies()) {
                try {
                    types.AddRange(assembly.GetTypes());
                } catch(ReflectionTypeLoadException ex) {
                    types.AddRange(ex.Types.Where(type => type != null));
                }
            }

            return types.ToArray();
        }

        public static Type[] GetNonAbstractTypes<T>(this AppDomain appDomain) {
            return GetAllTypes(appDomain)
                    .Where(type => !type.IsAbstract)
                    .Where(type => type.ImplementsInterface<T>())
                    .ToArray();
        }

        public static T[] GetInstancesOf<T>(this AppDomain appDomain) {
            return GetNonAbstractTypes<T>(appDomain)
                    .Select(type => (T)Activator.CreateInstance(type))
                    .ToArray();
        }
    }
}
