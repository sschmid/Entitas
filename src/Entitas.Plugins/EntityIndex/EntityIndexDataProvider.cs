using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jenny;
using Jenny.Generator;
using DesperateDevs.Extensions;
using DesperateDevs.Reflection;
using DesperateDevs.Serialization;
using Entitas.Plugins.Attributes;

namespace Entitas.Plugins
{
    public class EntityIndexDataProvider : IDataProvider, IConfigurable, ICachable, IDoctor
    {
        public string Name => "Entity Index";
        public int Order => 0;
        public bool RunInDryMode => true;

        public Dictionary<string, string> DefaultProperties =>
            _assembliesConfig.DefaultProperties
                .Merge(_ignoreNamespacesConfig.DefaultProperties)
                .Merge(_contextConfig.DefaultProperties);

        public Dictionary<string, object> ObjectCache { get; set; }

        readonly CodeGeneratorConfig _codeGeneratorConfig = new CodeGeneratorConfig();
        readonly AssembliesConfig _assembliesConfig = new AssembliesConfig();
        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();
        readonly ContextConfig _contextConfig = new ContextConfig();

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
            _contextConfig.Configure(preferences);
        }

        public CodeGeneratorData[] GetData()
        {
            var types = _types ?? PluginUtil
                .GetCachedAssemblyResolver(ObjectCache, _assembliesConfig.Assemblies, _codeGeneratorConfig.SearchPaths)
                .GetTypes().ToArray();

            var entityIndexData = types
                .Where(type => !type.IsAbstract)
                .Where(type => type.ImplementsInterface<IComponent>())
                .ToDictionary(
                    type => type,
                    type => type.GetPublicMemberInfos())
                .Where(kvp => kvp.Value.Any(info => info.Attributes.Any(attr => attr.Attribute is AbstractEntityIndexAttribute)))
                .SelectMany(kvp => CreateEntityIndexData(kvp.Key, kvp.Value));

            var customEntityIndexData = types
                .Where(type => !type.IsAbstract)
                .Where(type => Attribute.IsDefined(type, typeof(CustomEntityIndexAttribute)))
                .Select(CreateCustomEntityIndexData);

            return entityIndexData.Concat(customEntityIndexData).ToArray();
        }

        EntityIndexData[] CreateEntityIndexData(Type type, PublicMemberInfo[] infos)
        {
            var hasMultiple = infos.Count(i => i.Attributes.Count(attr => attr.Attribute is AbstractEntityIndexAttribute) == 1) > 1;
            return infos
                .Where(i => i.Attributes.Count(attr => attr.Attribute is AbstractEntityIndexAttribute) == 1)
                .Select(info =>
                {
                    var data = new EntityIndexData();
                    var attribute = (AbstractEntityIndexAttribute)info.Attributes.Single(attr => attr.Attribute is AbstractEntityIndexAttribute).Attribute;
                    data.Type = GetEntityIndexType(attribute);
                    data.IsCustom = false;
                    data.Name = type.ToCompilableString().ToComponentName(_ignoreNamespacesConfig.IgnoreNamespaces);
                    data.KeyType = info.Type.ToCompilableString();
                    data.ComponentType = type.ToCompilableString();
                    data.MemberName = info.Name;
                    data.HasMultiple = hasMultiple;
                    var contexts = Attribute.GetCustomAttributes(type).OfType<ContextAttribute>().Select(attr => attr.Name).ToArray();
                    if (contexts.Length == 0)
                        contexts = new[] {_contextConfig.Contexts[0]};
                    data.Contexts = contexts;
                    return data;
                }).ToArray();
        }

        EntityIndexData CreateCustomEntityIndexData(Type type)
        {
            var data = new EntityIndexData();
            var attribute = (CustomEntityIndexAttribute)type.GetCustomAttributes(typeof(CustomEntityIndexAttribute), false)[0];
            data.Type = type.ToCompilableString();
            data.IsCustom = true;
            data.Name = type.ToCompilableString().RemoveDots();
            data.HasMultiple = false;
            data.Contexts = new[] {attribute.Type.ToCompilableString().ShortTypeName().RemoveContextSuffix()};
            data.CustomMethods = type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => Attribute.IsDefined(method, typeof(EntityIndexGetMethodAttribute)))
                .Select(method => new MethodData(
                    method.ReturnType.ToCompilableString(),
                    method.Name,
                    method.GetParameters()
                        .Select(p => new MemberData(p.ParameterType.ToCompilableString(), p.Name))
                        .ToArray()
                )).ToArray();

            return data;
        }

        string GetEntityIndexType(AbstractEntityIndexAttribute attribute) => attribute.Type switch
        {
            EntityIndexType.EntityIndex => "Entitas.EntityIndex",
            EntityIndexType.PrimaryEntityIndex => "Entitas.PrimaryEntityIndex",
            _ => throw new Exception($"Unhandled EntityIndexType: {attribute.Type}")
        };

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
                        $"{typeName} loads and reflects {string.Join(", ", _assembliesConfig.Assemblies)} and therefore doesn't support server mode!",
                        $"Don't use the code generator in server mode with {typeName}",
                        DiagnosisSeverity.Hint
                    );
                }
            }

            return Diagnosis.Healthy;
        }

        public bool ApplyFix() => false;
    }
}
