using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entitas.CodeGeneration.Attributes;
using Entitas.CodeGeneration.CodeGenerator;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class EntityIndexDataProvider : ICodeGeneratorDataProvider, IConfigurable {

        public string name { get { return "Entity Index"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        const string IGNORE_NAMESPACES_KEY = "Entitas.CodeGeneration.Plugins.IgnoreNamespaces";

        public Dictionary<string, string> defaultProperties {
            get { return new Dictionary<string, string> { { IGNORE_NAMESPACES_KEY, "false" } }; }
        }

        bool ignoreNamespaces { get { return properties[IGNORE_NAMESPACES_KEY] == "true"; } }

        Dictionary<string, string> properties {
            get {
                if(_properties == null) {
                    _properties = defaultProperties;
                }

                return _properties;
            }
        }

        Dictionary<string, string> _properties;
        Type[] _types;

        public EntityIndexDataProvider() : this(null) {
        }

        public EntityIndexDataProvider(Type[] types) {
            _types = types;
        }

        public void Configure(Dictionary<string, string> properties) {
            _properties = properties;
        }

        public CodeGeneratorData[] GetData() {
            if(_types == null) {
                _types = CodeGeneratorUtil.LoadTypesFromAssemblies();
            }

            var entityIndexData = _types
                .Where(type => !type.IsAbstract)
                .Where(type => type.ImplementsInterface<IComponent>())
                .ToDictionary(
                    type => type,
                    type => type.GetPublicMemberInfos())
                .Where(kv => kv.Value.Any(info => info.attributes.Any(attr => attr.attribute is AbstractEntityIndexAttribute)))
                .Select(kv => createEntityIndexData(kv.Key, kv.Value));

            var customEntityIndexData = _types
                .Where(type => !type.IsAbstract)
                .Where(type => Attribute.IsDefined(type, typeof(CustomEntityIndexAttribute)))
                .Select(type => createCustomEntityIndexData(type));

            return entityIndexData
                .Concat(customEntityIndexData)
                .ToArray();
        }

        EntityIndexData createEntityIndexData(Type type, List<PublicMemberInfo> infos) {
            var data = new EntityIndexData();

            var info = infos.Single(i => i.attributes.Count(attr => attr.attribute is AbstractEntityIndexAttribute) == 1);
            var attribute = (AbstractEntityIndexAttribute)info.attributes.Single(attr => attr.attribute is AbstractEntityIndexAttribute).attribute;

            data.SetEntityIndexType(getEntityIndexType(attribute));
            data.IsCustom(false);
            data.SetEntityIndexName(type.ToCompilableString().ToComponentName(ignoreNamespaces));
            data.SetKeyType(info.type.ToCompilableString());
            data.SetComponentType(type.ToCompilableString());
            data.SetMemberName(info.name);
            data.SetContextNames(ContextsComponentDataProvider.GetContextNamesOrDefault(type));

            return data;
        }

        EntityIndexData createCustomEntityIndexData(Type type) {
            var data = new EntityIndexData();

            var attribute = (CustomEntityIndexAttribute)type.GetCustomAttributes(typeof(CustomEntityIndexAttribute), false)[0];

            data.SetEntityIndexType(type.ToCompilableString());
            data.IsCustom(true);
            data.SetEntityIndexName(type.ToCompilableString().RemoveDots());

            data.SetContextNames(new [] { attribute.contextType.ToCompilableString().ShortTypeName().RemoveContextSuffix() });

            var getMethods = type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => Attribute.IsDefined(method, typeof(EntityIndexGetMethodAttribute )))
                .Select(method => new MethodData (
                    method.ReturnType.ToCompilableString(),
                    method.Name,
                    method.GetParameters()
                        .Select(p => new MemberData(p.ParameterType.ToCompilableString(), p.Name))
                        .ToArray()
                )).ToArray();

            data.SetCustomMethods(getMethods);

            return data;
        }

        string getEntityIndexType(AbstractEntityIndexAttribute attribute) {
            switch(attribute.entityIndexType) {
                case EntityIndexType.EntityIndex:
                    return "Entitas.EntityIndex";
                case EntityIndexType.PrimaryEntityIndex:
                    return "Entitas.PrimaryEntityIndex";
                default:
                    throw new Exception("Unhandled EntityIndexType: " + attribute.entityIndexType);
            }
        }
    }

    public static class EntityIndexDataExtension {

        public const string ENTITY_INDEX_TYPE = "entityIndex_type";

        public const string ENTITY_INDEX_IS_CUSTOM = "entityIndex_isCustom";
        public const string ENTITY_INDEX_CUSTOM_METHODS = "entityIndex_customMethods";

        public const string ENTITY_INDEX_NAME = "entityIndex_name";
        public const string ENTITY_INDEX_CONTEXT_NAMES = "entityIndex_contextNames";

        public const string ENTITY_INDEX_KEY_TYPE = "entityIndex_keyType";
        public const string ENTITY_INDEX_COMPONENT_TYPE = "entityIndex_componentType";
        public const string ENTITY_INDEX_MEMBER_NAME = "entityIndex_memberName";

        public static string GetEntityIndexType(this EntityIndexData data) {
            return (string)data[ENTITY_INDEX_TYPE];
        }

        public static void SetEntityIndexType(this EntityIndexData data, string type) {
            data[ENTITY_INDEX_TYPE] = type;
        }

        public static bool IsCustom(this EntityIndexData data) {
            return (bool)data[ENTITY_INDEX_IS_CUSTOM];
        }

        public static void IsCustom(this EntityIndexData data, bool isCustom) {
            data[ENTITY_INDEX_IS_CUSTOM] = isCustom;
        }

        public static MethodData[] GetCustomMethods(this EntityIndexData data) {
            return (MethodData[])data[ENTITY_INDEX_CUSTOM_METHODS];
        }

        public static void SetCustomMethods(this EntityIndexData data, MethodData[] methods) {
            data[ENTITY_INDEX_CUSTOM_METHODS] = methods;
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

        public static string GetMemberName(this EntityIndexData data) {
            return (string)data[ENTITY_INDEX_MEMBER_NAME];
        }

        public static void SetMemberName(this EntityIndexData data, string memberName) {
            data[ENTITY_INDEX_MEMBER_NAME] = memberName;
        }
    }
}
