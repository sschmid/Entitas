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
            return types
                .Where(type => type.GetInterfaces().Contains(typeof(IComponent)))
                .Select(createComponentInfo)
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
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Select(field => new ComponentFieldInfo(field.FieldType.ToCompilableString(), field.Name))
                .ToArray();
        }

        static string[] getPools(Type type) {
            return Attribute.GetCustomAttributes(type)
                .OfType<PoolAttribute>()
                .Select(attr => attr.poolName)
                .OrderBy(poolName => poolName)
                .ToArray();
        }

        static bool getIsSingleEntity(Type type) {
            return Attribute.GetCustomAttributes(type)
                .Any(attr => attr is SingleEntityAttribute);
        }

        static string getSingleComponentPrefix(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                .OfType<CustomPrefixAttribute>()
                .SingleOrDefault();

            return attr == null ? "is" : attr.prefix;
        }

        static bool getGenerateMethods(Type type) {
            return !Attribute.GetCustomAttributes(type)
                .Any(attr => attr is DontGenerateAttribute);
        }

        static bool getGenerateIndex(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                .OfType<DontGenerateAttribute>()
                .SingleOrDefault();

            return attr == null || attr.generateIndex;
        }
    }
}
    