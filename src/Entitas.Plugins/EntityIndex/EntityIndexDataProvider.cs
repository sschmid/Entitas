using System;
using System.Collections.Generic;
using System.Linq;
using Jenny;
using Jenny.Plugins;
using DesperateDevs.Extensions;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.Plugins.Attributes;
using Microsoft.CodeAnalysis;

namespace Entitas.Plugins
{
    public class EntityIndexDataProvider : IDataProvider, IConfigurable, ICachable
    {
        public string Name => "Entity Index (Roslyn)";
        public int Order => 0;
        public bool RunInDryMode => true;

        public Dictionary<string, string> DefaultProperties =>
            _projectPathConfig.DefaultProperties
                .Merge(_ignoreNamespacesConfig.DefaultProperties)
                .Merge(_contextConfig.DefaultProperties);

        public Dictionary<string, object> ObjectCache { get; set; }

        readonly ProjectPathConfig _projectPathConfig = new ProjectPathConfig();
        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();
        readonly ContextConfig _contextConfig = new ContextConfig();

        readonly INamedTypeSymbol[] _types;

        public EntityIndexDataProvider() : this(null) { }

        public EntityIndexDataProvider(INamedTypeSymbol[] types) => _types = types;

        public void Configure(Preferences preferences)
        {
            _projectPathConfig.Configure(preferences);
            _ignoreNamespacesConfig.Configure(preferences);
            _contextConfig.Configure(preferences);
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
                .Where(kvp => kvp.Value.Any(symbol => symbol.GetAttribute<AbstractEntityIndexAttribute>(true) != null))
                .SelectMany(kvp => CreateEntityIndexData(kvp.Key, kvp.Value));

            var customEntityIndexData = types
                .Where(type => !type.IsAbstract)
                .Where(type => type.GetAttribute<CustomEntityIndexAttribute>() != null)
                .Select(CreateCustomEntityIndexData);

            return entityIndexData.Concat(customEntityIndexData).ToArray();
        }

        EntityIndexData[] CreateEntityIndexData(INamedTypeSymbol type, ISymbol[] members)
        {
            var hasMultiple = members.Count(member => member.GetAttribute<AbstractEntityIndexAttribute>(true) != null) > 1;
            return members
                .Where(member => member.GetAttribute<AbstractEntityIndexAttribute>(true) != null)
                .Select(member =>
                {
                    var data = new EntityIndexData();
                    var attribute = member.GetAttribute<AbstractEntityIndexAttribute>(true);
                    data.Type = GetEntityIndexType(attribute);
                    data.IsCustom = false;
                    data.Name = type.ToCompilableString().ToComponentName(_ignoreNamespacesConfig.IgnoreNamespaces);
                    data.KeyType = member.PublicMemberType().ToCompilableString();
                    data.ComponentType = type.ToCompilableString();
                    data.MemberName = member.Name;
                    data.HasMultiple = hasMultiple;
                    var contexts = ComponentDataProvider.GetContexts(type, _contextConfig.Contexts);
                    if (contexts.Length == 0)
                        contexts = new[] {_contextConfig.Contexts[0]};
                    data.Contexts = contexts;
                    return data;
                }).ToArray();
        }

        EntityIndexData CreateCustomEntityIndexData(INamedTypeSymbol type)
        {
            var data = new EntityIndexData();
            var attribute = type.GetAttribute<CustomEntityIndexAttribute>();
            data.Type = type.ToCompilableString();
            data.IsCustom = true;
            data.Name = type.ToCompilableString().RemoveDots();
            data.HasMultiple = false;
            data.Contexts = new[] {((INamedTypeSymbol)attribute.ConstructorArguments.First().Value).ToCompilableString().ShortTypeName().RemoveContextSuffix()};

            data.CustomMethods = type
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

            return data;
        }

        string GetEntityIndexType(AttributeData attribute)
        {
            var entityIndexType = attribute.ToString();
            return entityIndexType switch
            {
                "Entitas.Plugins.Attributes.EntityIndexAttribute" => "Entitas.EntityIndex",
                "Entitas.Plugins.Attributes.PrimaryEntityIndexAttribute" => "Entitas.PrimaryEntityIndex",
                _ => throw new Exception($"Unhandled EntityIndexType: {entityIndexType}")
            };
        }
    }
}
