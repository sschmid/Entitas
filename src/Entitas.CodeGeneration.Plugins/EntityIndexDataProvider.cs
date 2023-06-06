using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jenny;
using Jenny.Generator;
using DesperateDevs.Extensions;
using DesperateDevs.Reflection;
using DesperateDevs.Serialization;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins
{
    public class EntityIndexDataProvider : IDataProvider, IConfigurable, ICachable, IDoctor
    {
        public string Name => "Entity Index";
        public int Order => 0;
        public bool RunInDryMode => true;

        public Dictionary<string, string> DefaultProperties =>
            _assembliesConfig.DefaultProperties.Merge(new[]
            {
                _ignoreNamespacesConfig.DefaultProperties,
                _contextsComponentDataProvider.DefaultProperties
            });

        public Dictionary<string, object> ObjectCache { get; set; }

        readonly CodeGeneratorConfig _codeGeneratorConfig = new CodeGeneratorConfig();
        readonly AssembliesConfig _assembliesConfig = new AssembliesConfig();
        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();
        readonly ContextsComponentDataProvider _contextsComponentDataProvider = new ContextsComponentDataProvider();

        readonly Type[] _types;

        public EntityIndexDataProvider() : this(null) { }

        public EntityIndexDataProvider(Type[] types)
        {
            _types = types;
        }

        public void Configure(Preferences preferences)
        {
            _codeGeneratorConfig.Configure(preferences);
            _assembliesConfig.Configure(preferences);
            _ignoreNamespacesConfig.Configure(preferences);
            _contextsComponentDataProvider.Configure(preferences);
        }

        public CodeGeneratorData[] GetData()
        {
            var types = _types ?? PluginUtil
                .GetCachedAssemblyResolver(ObjectCache, _assembliesConfig.assemblies, _codeGeneratorConfig.SearchPaths)
                .GetTypes().ToArray();

            var entityIndexData = types
                .Where(type => !type.IsAbstract)
                .Where(type => type.ImplementsInterface<IComponent>())
                .ToDictionary(
                    type => type,
                    type => type.GetPublicMemberInfos())
                .Where(kv => kv.Value.Any(info => info.Attributes.Any(attr => attr.Attribute is AbstractEntityIndexAttribute)))
                .SelectMany(kv => createEntityIndexData(kv.Key, kv.Value));

            var customEntityIndexData = types
                .Where(type => !type.IsAbstract)
                .Where(type => Attribute.IsDefined(type, typeof(CustomEntityIndexAttribute)))
                .Select(createCustomEntityIndexData);

            return entityIndexData
                .Concat(customEntityIndexData)
                .ToArray();
        }

        EntityIndexData[] createEntityIndexData(Type type, PublicMemberInfo[] infos)
        {
            var hasMultiple = infos.Count(i => i.Attributes.Count(attr => attr.Attribute is AbstractEntityIndexAttribute) == 1) > 1;
            return infos
                .Where(i => i.Attributes.Count(attr => attr.Attribute is AbstractEntityIndexAttribute) == 1)
                .Select(info =>
                {
                    var data = new EntityIndexData();
                    var attribute = (AbstractEntityIndexAttribute)info.Attributes.Single(attr => attr.Attribute is AbstractEntityIndexAttribute).Attribute;

                    data.SetEntityIndexType(getEntityIndexType(attribute));
                    data.IsCustom(false);
                    data.SetEntityIndexName(type.ToCompilableString().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces));
                    data.SetKeyType(info.Type.ToCompilableString());
                    data.SetComponentType(type.ToCompilableString());
                    data.SetMemberName(info.Name);
                    data.SetHasMultiple(hasMultiple);
                    data.SetContextNames(_contextsComponentDataProvider.GetContextNamesOrDefault(type));

                    return data;
                }).ToArray();
        }

        EntityIndexData createCustomEntityIndexData(Type type)
        {
            var data = new EntityIndexData();
            var attribute = (CustomEntityIndexAttribute)type.GetCustomAttributes(typeof(CustomEntityIndexAttribute), false)[0];
            data.SetEntityIndexType(type.ToCompilableString());
            data.IsCustom(true);
            data.SetEntityIndexName(type.ToCompilableString().RemoveDots());
            data.SetHasMultiple(false);
            data.SetContextNames(new[] {attribute.contextType.ToCompilableString().TypeName().RemoveContextSuffix()});

            var getMethods = type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => Attribute.IsDefined(method, typeof(EntityIndexGetMethodAttribute)))
                .Select(method => new MethodData(
                    method.ReturnType.ToCompilableString(),
                    method.Name,
                    method.GetParameters()
                        .Select(p => new MemberData(p.ParameterType.ToCompilableString(), p.Name))
                        .ToArray()
                )).ToArray();

            data.SetCustomMethods(getMethods);

            return data;
        }

        string getEntityIndexType(AbstractEntityIndexAttribute attribute)
        {
            switch (attribute.entityIndexType)
            {
                case EntityIndexType.EntityIndex:
                    return "Entitas.EntityIndex";
                case EntityIndexType.PrimaryEntityIndex:
                    return "Entitas.PrimaryEntityIndex";
                default:
                    throw new Exception($"Unhandled EntityIndexType: {attribute.entityIndexType}");
            }
        }

        public Diagnosis Diagnose()
        {
            var isStandalone = AppDomain.CurrentDomain
                .GetAllTypes()
                .Any(type => type.FullName.StartsWith("Jenny.Generator.Cli"));

            if (isStandalone)
            {
                var typeName = typeof(EntityIndexDataProvider).FullName;
                if (_codeGeneratorConfig.DataProviders.Contains(typeName))
                {
                    return new Diagnosis(
                        $"{typeName} loads and reflects {string.Join(", ", _assembliesConfig.assemblies)} and therefore doesn't support server mode!",
                        $"Don't use the code generator in server mode with {typeName}",
                        DiagnosisSeverity.Hint
                    );
                }
            }

            return Diagnosis.Healthy;
        }

        public bool ApplyFix() => false;
    }

    public static class EntityIndexDataExtension
    {
        public const string ENTITY_INDEX_TYPE = "EntityIndex.Type";
        public static string GetEntityIndexType(this EntityIndexData data) => (string)data[ENTITY_INDEX_TYPE];
        public static void SetEntityIndexType(this EntityIndexData data, string type) => data[ENTITY_INDEX_TYPE] = type;

        public const string ENTITY_INDEX_IS_CUSTOM = "EntityIndex.Custom";
        public static bool IsCustom(this EntityIndexData data) => (bool)data[ENTITY_INDEX_IS_CUSTOM];
        public static void IsCustom(this EntityIndexData data, bool isCustom) => data[ENTITY_INDEX_IS_CUSTOM] = isCustom;

        public const string ENTITY_INDEX_CUSTOM_METHODS = "EntityIndex.CustomMethods";
        public static MethodData[] GetCustomMethods(this EntityIndexData data) => (MethodData[])data[ENTITY_INDEX_CUSTOM_METHODS];
        public static void SetCustomMethods(this EntityIndexData data, MethodData[] methods) => data[ENTITY_INDEX_CUSTOM_METHODS] = methods;

        public const string ENTITY_INDEX_NAME = "EntityIndex.Name";
        public static string GetEntityIndexName(this EntityIndexData data) => (string)data[ENTITY_INDEX_NAME];
        public static void SetEntityIndexName(this EntityIndexData data, string name) => data[ENTITY_INDEX_NAME] = name;

        public const string ENTITY_INDEX_CONTEXT_NAMES = "EntityIndex.ContextNames";
        public static string[] GetContextNames(this EntityIndexData data) => (string[])data[ENTITY_INDEX_CONTEXT_NAMES];
        public static void SetContextNames(this EntityIndexData data, string[] contextNames) => data[ENTITY_INDEX_CONTEXT_NAMES] = contextNames;

        public const string ENTITY_INDEX_KEY_TYPE = "EntityIndex.KeyType";
        public static string GetKeyType(this EntityIndexData data) => (string)data[ENTITY_INDEX_KEY_TYPE];
        public static void SetKeyType(this EntityIndexData data, string type) => data[ENTITY_INDEX_KEY_TYPE] = type;

        public const string ENTITY_INDEX_COMPONENT_TYPE = "EntityIndex.ComponentType";
        public static string GetComponentType(this EntityIndexData data) => (string)data[ENTITY_INDEX_COMPONENT_TYPE];
        public static void SetComponentType(this EntityIndexData data, string type) => data[ENTITY_INDEX_COMPONENT_TYPE] = type;

        public const string ENTITY_INDEX_MEMBER_NAME = "EntityIndex.MemberName";
        public static string GetMemberName(this EntityIndexData data) => (string)data[ENTITY_INDEX_MEMBER_NAME];
        public static void SetMemberName(this EntityIndexData data, string memberName) => data[ENTITY_INDEX_MEMBER_NAME] = memberName;

        public const string ENTITY_INDEX_HAS_MULTIPLE = "EntityIndex.HasMultiple";
        public static bool GetHasMultiple(this EntityIndexData data) => (bool)data[ENTITY_INDEX_HAS_MULTIPLE];
        public static void SetHasMultiple(this EntityIndexData data, bool hasMultiple) => data[ENTITY_INDEX_HAS_MULTIPLE] = hasMultiple;
    }
}
