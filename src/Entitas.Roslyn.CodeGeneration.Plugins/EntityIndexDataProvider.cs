using System;
using System.Collections.Generic;
using System.Linq;
using Jenny;
using Jenny.Plugins;
using DesperateDevs.Extensions;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.CodeGeneration.Attributes;
using Entitas.CodeGeneration.Plugins;
using Microsoft.CodeAnalysis;

namespace Entitas.Roslyn.CodeGeneration.Plugins
{
    public class EntityIndexDataProvider : IDataProvider, IConfigurable, ICachable
    {
        public string Name => "Entity Index (Roslyn)";
        public int Order => 0;
        public bool RunInDryMode => true;

        public Dictionary<string, string> DefaultProperties =>
            _ignoreNamespacesConfig.DefaultProperties.Merge(new[]
            {
                _projectPathConfig.DefaultProperties,
                _contextsComponentDataProvider.DefaultProperties
            });

        public Dictionary<string, object> ObjectCache { get; set; }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();
        readonly ProjectPathConfig _projectPathConfig = new ProjectPathConfig();
        readonly ContextsComponentDataProvider _contextsComponentDataProvider = new ContextsComponentDataProvider();

        readonly INamedTypeSymbol[] _types;

        public EntityIndexDataProvider() : this(null) { }

        public EntityIndexDataProvider(INamedTypeSymbol[] types)
        {
            _types = types;
        }

        public void Configure(Preferences preferences)
        {
            _ignoreNamespacesConfig.Configure(preferences);
            _projectPathConfig.Configure(preferences);
            _contextsComponentDataProvider.Configure(preferences);
        }

        public CodeGeneratorData[] GetData()
        {
            var types = _types ?? Jenny.Plugins.Roslyn.PluginUtil
                .GetCachedProjectParser(ObjectCache, _projectPathConfig.ProjectPath)
                .GetTypes();

            var componentInterface = typeof(IComponent).ToCompilableString();

            var entityIndexData = types
                .Where(type => type.AllInterfaces.Any(i => i.ToCompilableString() == componentInterface))
                .Where(type => !type.IsAbstract)
                .ToDictionary(
                    type => type,
                    type => type.GetPublicMembers(true))
                .Where(kv => kv.Value.Any(symbol => symbol.GetAttribute<AbstractEntityIndexAttribute>(true) != null))
                .SelectMany(kv => createEntityIndexData(kv.Key, kv.Value));

            var customEntityIndexData = types
                .Where(type => !type.IsAbstract)
                .Where(type => type.GetAttribute<CustomEntityIndexAttribute>() != null)
                .Select(createCustomEntityIndexData);

            return entityIndexData
                .Concat(customEntityIndexData)
                .ToArray();
        }

        EntityIndexData[] createEntityIndexData(INamedTypeSymbol type, ISymbol[] members)
        {
            var hasMultiple = members.Count(member => member.GetAttribute<AbstractEntityIndexAttribute>(true) != null) > 1;
            return members
                .Where(member => member.GetAttribute<AbstractEntityIndexAttribute>(true) != null)
                .Select(member =>
                {
                    var data = new EntityIndexData();
                    var attribute = member.GetAttribute<AbstractEntityIndexAttribute>(true);

                    data.SetEntityIndexType(getEntityIndexType(attribute));
                    data.IsCustom(false);
                    data.SetEntityIndexName(type.ToCompilableString().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces));
                    data.SetHasMultiple(hasMultiple);
                    data.SetKeyType(member.PublicMemberType().ToCompilableString());
                    data.SetComponentType(type.ToCompilableString());
                    data.SetMemberName(member.Name);
                    data.SetContextNames(_contextsComponentDataProvider.GetContextNamesOrDefault(type));

                    return data;
                }).ToArray();
        }

        EntityIndexData createCustomEntityIndexData(INamedTypeSymbol type)
        {
            var data = new EntityIndexData();
            var attribute = type.GetAttribute<CustomEntityIndexAttribute>();
            data.SetEntityIndexType(type.ToCompilableString());
            data.IsCustom(true);
            data.SetEntityIndexName(type.ToCompilableString().RemoveDots());
            data.SetHasMultiple(false);
            data.SetContextNames(new[]
            {
                ((INamedTypeSymbol)attribute.ConstructorArguments.First().Value)
                .ToCompilableString()
                .TypeName()
                .RemoveContextSuffix()
            });

            var getMethods = type
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(method => method.DeclaredAccessibility == Accessibility.Public)
                .Where(method => !method.IsStatic)
                .Where(method => method.GetAttribute<EntityIndexGetMethodAttribute>() != null)
                .Select(method => new MethodData(
                    method.ReturnType.ToCompilableString(),
                    method.Name,
                    method.Parameters
                        .Select(p => new MemberData(p.ToCompilableString(), p.Name))
                        .ToArray()
                ))
                .ToArray();

            data.SetCustomMethods(getMethods);

            return data;
        }

        string getEntityIndexType(AttributeData attribute)
        {
            var entityIndexType = attribute.ToString();
            switch (entityIndexType)
            {
                case "Entitas.CodeGeneration.Attributes.EntityIndexAttribute":
                    return "Entitas.EntityIndex";
                case "Entitas.CodeGeneration.Attributes.PrimaryEntityIndexAttribute":
                    return "Entitas.PrimaryEntityIndex";
                default:
                    throw new Exception($"Unhandled EntityIndexType: {entityIndexType}");
            }
        }
    }
}
