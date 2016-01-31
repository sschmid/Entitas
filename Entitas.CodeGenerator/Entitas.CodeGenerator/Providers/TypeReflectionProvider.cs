using System;
using System.Linq;
using System.Reflection;

namespace Entitas.CodeGenerator {
    public class TypeReflectionProvider : ICodeGeneratorDataProvider {

        public string[] poolNames { get { return _poolNames; } }
        public ComponentInfo[] componentInfos { get { return _componentInfos; } }

        readonly string[] _poolNames;
        readonly ComponentInfo[] _componentInfos;

        public TypeReflectionProvider(Type[] types) {
            _poolNames = GetPoolNames(types);
            _componentInfos = GetComponentInfos(types);
        }

        public static string[] GetPoolNames(Type[] types) {
            return types
                .Where(type => type.IsSubclassOf(typeof(PoolAttribute)))
                .Select(type => type.Name)
                .Select(name => name.Replace(typeof(PoolAttribute).Name, string.Empty))
                .ToArray();
        }

        public static ComponentInfo[] GetComponentInfos(Type[] types) {
            return types
                .Where(type => type.GetInterfaces().Any(i => i.FullName == "Entitas.IComponent"))
                .Select(type => CreateComponentInfo(type))
                .ToArray();
        }

        public static ComponentInfo CreateComponentInfo(Type type) {
            return new ComponentInfo(
                type.ToCompilableString(),
                GetFieldInfos(type),
                GetPools(type),
                GetIsSingleEntity(type),
                GetSingleComponentPrefix(type),
                GetGenerateMethods(type),
                GetGenerateIndex(type)
            );
        }

        public static ComponentFieldInfo[] GetFieldInfos(Type type) {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Select(field => new ComponentFieldInfo(field.FieldType.ToCompilableString(), field.Name))
                .ToArray();
        }

        public static string[] GetPools(Type type) {
            return Attribute.GetCustomAttributes(type)
                .OfType<PoolAttribute>()
                .Select(attr => attr.poolName)
                .OrderBy(poolName => poolName)
                .ToArray();
        }

        public static bool GetIsSingleEntity(Type type) {
            return Attribute.GetCustomAttributes(type)
                .Any(attr => attr is SingleEntityAttribute);
        }

        public static string GetSingleComponentPrefix(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                .OfType<CustomPrefixAttribute>()
                .SingleOrDefault();

            return attr == null ? "is" : attr.prefix;
        }

        public static bool GetGenerateMethods(Type type) {
            return !Attribute.GetCustomAttributes(type)
                .Any(attr => attr is DontGenerateAttribute);
        }

        public static bool GetGenerateIndex(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                .OfType<DontGenerateAttribute>()
                .SingleOrDefault();

            return attr == null || attr.generateIndex;
        }
    }
}
    