using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {
    public class TypeReflectionProvider : ICodeGeneratorDataProvider {

        public string[] poolNames { get { return _poolNames; } }
        public ComponentInfo[] componentInfos { get { return _componentInfos; } }

        readonly string[] _poolNames;
        readonly ComponentInfo[] _componentInfos;

        public TypeReflectionProvider(Type[] types) {
            _poolNames = getPoolNames(types);
            _componentInfos = getComponentInfos(types);
        }

        static string[] getPoolNames(Type[] types) {
            return types
                .Where(type => type.IsSubclassOf(typeof(PoolAttribute)))
                .Select(type => type.Name)
                .Select(name => name.Replace(typeof(PoolAttribute).Name, string.Empty))
                .ToArray();
        }

        static ComponentInfo[] getComponentInfos(Type[] types) {
            return getComponents(types)
                .Select(createComponentInfo)
                .ToArray();
        }

        static Type[] getComponents(Type[] types) {
            return types
                .Where(type => type.GetInterfaces().Contains(typeof(IComponent)))
                .ToArray();
        }

        static ComponentInfo createComponentInfo(Type type) {
            return new ComponentInfo(
                type.ToCompilableString(),
                getFieldInfos(type),
                getPools(type),
                getIsSingleEntity(type),
                getSingleComponentPrefix(type),
                getGenerateMethods(type),
                getGenerateIndex(type)
            );
        }

        static ComponentFieldInfo[] getFieldInfos(Type type) {
            return new ComponentFieldInfo[0];
        }

        static string[] getPools(Type type) {
            return Attribute.GetCustomAttributes(type)
                .Where(attr => attr is PoolAttribute)
                .Select(attr => ((PoolAttribute)attr).poolName)
                .OrderBy(poolName => poolName)
                .ToArray();
        }

        static bool getIsSingleEntity(Type type) {
            return false;
        }

        static string getSingleComponentPrefix(Type type) {
            return "is";
        }

        static bool getGenerateMethods(Type type) {
            return true;
        }

        static bool getGenerateIndex(Type type) {
            return true;
        }
    }
}
    