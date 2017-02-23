using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entitas.CodeGenerator.Api;

namespace Entitas.CodeGenerator {

    public class EntityIndexDataProvider : ICodeGeneratorDataProvider {

        public string name { get { return "Entity Index"; } }
        public bool isEnabledByDefault { get { return true; } }

        Type[] _types;
        string _defaultContextName;

        public EntityIndexDataProvider()
            : this(Assembly.GetAssembly(typeof(IEntity)).GetTypes()) {
        }

        public EntityIndexDataProvider(Type[] types)
            : this(types, new CodeGeneratorConfig(EntitasPreferences.LoadConfig()).contexts[0]) {
        }

        public EntityIndexDataProvider(Type[] types, string defaultContextName) {
            _types = types;
            _defaultContextName = defaultContextName;
        }

        public CodeGeneratorData[] GetData() {
            return _types
                .Where(type => !type.IsAbstract)
                .ToDictionary(
                    type => type,
                    type => type.GetPublicMemberInfos())
                .Where(kv => kv.Value.Any(info => info.attributes.Any(attr => attr.attribute is AbstractEntityIndexAttribute)))
                .Select(kv => createData(kv.Key, kv.Value))
                .ToArray();
        }

        EntityIndexData createData(Type type, List<PublicMemberInfo> infos) {
            var data = new EntityIndexData();

            var info = infos.Single(i => i.attributes.Count(attr => attr.attribute is AbstractEntityIndexAttribute) == 1);
            var attribute = (AbstractEntityIndexAttribute)info.attributes.Single(attr => attr.attribute is AbstractEntityIndexAttribute).attribute;

            data.IsPrimary(attribute.isPrimary);
            data.SetEntityIndexType(attribute.type);
            data.SetEntityIndexName(type.ToCompilableString().ShortTypeName().RemoveComponentSuffix());
            data.SetKeyType(info.type.ToCompilableString());
            data.SetComponentType(type.ToCompilableString());
            data.SetComponentName(type.ToCompilableString().ShortTypeName().RemoveComponentSuffix());
            data.SetMemberName(info.name);

            var contextNames = ContextsComponentDataProvider.GetContextNames(type);
            if(contextNames.Length == 0) {
                contextNames = new [] { _defaultContextName };
            }
            data.SetContextNames(contextNames);

            return data;
        }
    }

    public static class EntityIndexDataExtension {

        public const string ENTITY_INDEX_IS_PRIMARY = "entityIndex_isPrimary";
        public const string ENTITY_INDEX_TYPE = "entityIndex_type";
        public const string ENTITY_INDEX_NAME = "entityIndex_name";
        public const string ENTITY_INDEX_CONTEXT_NAMES = "entityIndex_contextNames";
        public const string ENTITY_INDEX_KEY_TYPE = "entityIndex_keyType";
        public const string ENTITY_INDEX_COMPONENT_TYPE = "entityIndex_componentType";
        public const string ENTITY_INDEX_COMPONENT_NAME = "entityIndex_componentName";
        public const string ENTITY_INDEX_MEMBER_NAME = "entityIndex_memberName";

        public static bool IsPrimary(this EntityIndexData data) {
            return (bool)data[ENTITY_INDEX_IS_PRIMARY];
        }

        public static void IsPrimary(this EntityIndexData data, bool isPrimary) {
            data[ENTITY_INDEX_IS_PRIMARY] = isPrimary;
        }

        public static string GetEntityIndexType(this EntityIndexData data) {
            return (string)data[ENTITY_INDEX_TYPE];
        }

        public static void SetEntityIndexType(this EntityIndexData data, string type) {
            data[ENTITY_INDEX_TYPE] = type;
        }

        public static string GetEntityIndexName(this EntityIndexData data) {
            return (string)data[ENTITY_INDEX_NAME];
        }

        public static void SetEntityIndexName(this EntityIndexData data, string name) {
            data[ENTITY_INDEX_NAME] = name;
        }

        public static string[] GetContextNames(this EntityIndexData data) {
            return (string[])data[ENTITY_INDEX_CONTEXT_NAMES];
        }

        public static void SetContextNames(this EntityIndexData data, string[] contextNames) {
            data[ENTITY_INDEX_CONTEXT_NAMES] = contextNames;
        }

        public static string GetKeyType(this EntityIndexData data) {
            return (string)data[ENTITY_INDEX_KEY_TYPE];
        }

        public static void SetKeyType(this EntityIndexData data, string type) {
            data[ENTITY_INDEX_KEY_TYPE] = type;
        }

        public static string GetComponentType(this EntityIndexData data) {
            return (string)data[ENTITY_INDEX_COMPONENT_TYPE];
        }

        public static void SetComponentType(this EntityIndexData data, string type) {
            data[ENTITY_INDEX_COMPONENT_TYPE] = type;
        }

        public static string GetComponentName(this EntityIndexData data) {
            return (string)data[ENTITY_INDEX_COMPONENT_NAME];
        }

        public static void SetComponentName(this EntityIndexData data, string componentName) {
            data[ENTITY_INDEX_COMPONENT_NAME] = componentName;
        }

        public static string GetMemberName(this EntityIndexData data) {
            return (string)data[ENTITY_INDEX_MEMBER_NAME];
        }

        public static void SetMemberName(this EntityIndexData data, string memberName) {
            data[ENTITY_INDEX_MEMBER_NAME] = memberName;
        }
    }
}
