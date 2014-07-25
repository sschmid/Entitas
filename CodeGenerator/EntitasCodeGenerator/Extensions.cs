using System;

namespace Entitas.CodeGenerator {
    public static class Extensions {
        public static string RemoveComponentSuffix(this Type type) {
            var componentSuffix = EntitasCodeGenerator.componentSuffix;
            if (type.Name.EndsWith(componentSuffix))
                return type.Name.Substring(0, type.Name.Length - componentSuffix.Length);

            return type.Name;
        }
    }
}

