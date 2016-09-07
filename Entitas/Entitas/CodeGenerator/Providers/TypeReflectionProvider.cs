using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.Serialization;

namespace Entitas.CodeGenerator {

    public class TypeReflectionProvider : ICodeGeneratorDataProvider {

        public ComponentInfo[] componentInfos { get { return _componentInfos; } }
        public string[] poolNames { get { return _poolNames; } }
        public string[] blueprintNames { get { return _blueprintNames; } }

        readonly ComponentInfo[] _componentInfos;
        readonly string[] _poolNames;
        readonly string[] _blueprintNames;

        public TypeReflectionProvider(Type[] types, string[] poolNames, string[] blueprintNames) {
            var pools = new HashSet<string>(poolNames);
            if (poolNames.Length == 0) {
                pools.Add(CodeGenerator.DEFAULT_POOL_NAME);
            }
            _componentInfos = GetComponentInfos(types);
            _poolNames = pools
                .Select(poolName => poolName.UppercaseFirst())
                .OrderBy(poolName => poolName)
                .ToArray();
            _blueprintNames = blueprintNames;
        }

        public static ComponentInfo[] GetComponentInfos(params Type[] types) {
            var infosFromComponents = types
                .Where(type => !type.IsInterface)
                .Where(type => !type.IsAbstract)
                .Where(type => type.GetInterfaces().Any(i => i.FullName == "Entitas.IComponent"))
                .Select(type => CreateComponentInfo(type));

            var infosForNonComponents = types
                .Where(type => !type.IsGenericType)
                .Where(type => !type.GetInterfaces().Any(i => i.FullName == "Entitas.IComponent"))
                .Where(type => GetPools(type, false).Length > 0)
                .SelectMany(type => CreateComponentInfosForClass(type));

            var generatedComponentsLookup = infosForNonComponents.ToLookup(info => info.fullTypeName);

            return infosFromComponents
                .Where(info => !generatedComponentsLookup.Contains(info.fullTypeName))
                .Concat(infosForNonComponents)
                .ToArray();
        }

        public static ComponentInfo CreateComponentInfo(Type type) {
            return new ComponentInfo(
                type.ToCompilableString(),
                GetPublicMemberInfo(type),
                GetPools(type, true),
                GetIsSingleEntity(type),
                GetSingleComponentPrefix(type),
                false,
                GetGenerateMethods(type),
                GetGenerateIndex(type)
            );
        }

        public static ComponentInfo[] CreateComponentInfosForClass(Type type) {
            return GetComponentNames(type)
                .Select(componentName => new ComponentInfo(
                    componentName,
                    new List<PublicMemberInfo> {
                        new PublicMemberInfo(type, "value")
                    },
                    GetPools(type, false),
                    GetIsSingleEntity(type),
                    GetSingleComponentPrefix(type),
                    true,
                    GetGenerateMethods(type),
                    GetGenerateIndex(type)
                )).ToArray();
        }

        public static List<PublicMemberInfo> GetPublicMemberInfo(Type type) {
            return type.GetPublicMemberInfos();
        }

        public static string[] GetPools(Type type, bool defaultIfEmpty) {
            var pools = Attribute.GetCustomAttributes(type)
                .Where(attr => isTypeOrHasBaseType(attr.GetType(), "Entitas.CodeGenerator.PoolAttribute"))
                .Select(attr => attr.GetType().GetField("poolName").GetValue(attr) as string)
                .OrderBy(poolName => poolName)
                .ToArray();

            if (pools.Length == 0 && defaultIfEmpty) {
                return new[] { CodeGenerator.DEFAULT_POOL_NAME };
            }

            var defaultPoolIndex = Array.IndexOf(pools, CodeGenerator.DEFAULT_POOL_NAME);
            if (defaultPoolIndex != -1) {
                pools[defaultPoolIndex] = pools[0];
                pools[0] = CodeGenerator.DEFAULT_POOL_NAME;
            }

            return pools;
        }

        public static bool GetIsSingleEntity(Type type) {
            return Attribute.GetCustomAttributes(type)
                .Any(attr => attr.GetType().FullName == "Entitas.CodeGenerator.SingleEntityAttribute");
        }

        public static string GetSingleComponentPrefix(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                .SingleOrDefault(a => isTypeOrHasBaseType(a.GetType(), "Entitas.CodeGenerator.CustomPrefixAttribute"));

            return attr == null ? "is" : (string)attr.GetType().GetField("prefix").GetValue(attr);
        }

        public static string[] GetComponentNames(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                .SingleOrDefault(a => isTypeOrHasBaseType(a.GetType(), "Entitas.CodeGenerator.CustomComponentNameAttribute"));

            if (attr == null) {
                var nameSplit = type.ToCompilableString().Split('.');
                var componentName = nameSplit[nameSplit.Length - 1].AddComponentSuffix();
                return new [] { componentName };
            }

            return (string[])attr.GetType().GetField("componentNames").GetValue(attr);
        }

        public static bool GetGenerateMethods(Type type) {
            return Attribute.GetCustomAttributes(type)
                .All(attr => attr.GetType().FullName != "Entitas.CodeGenerator.DontGenerateAttribute");
        }

        public static bool GetGenerateIndex(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                .SingleOrDefault(a => isTypeOrHasBaseType(a.GetType(), "Entitas.CodeGenerator.DontGenerateAttribute"));

            return attr == null || (bool)attr.GetType().GetField("generateIndex").GetValue(attr);
        }

        static bool hasBaseType(Type type, string fullTypeName) {
            if (type.FullName == fullTypeName) {
                return false;
            }

            return isTypeOrHasBaseType(type, fullTypeName);
        }

        static bool isTypeOrHasBaseType(Type type, string fullTypeName) {
            var t = type;
            while (t != null) {
                if (t.FullName == fullTypeName) {
                    return true;
                }
                t = t.BaseType;
            }

            return false;
        }
    }
}
