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
            _projectPathConfig.DefaultProperties.Merge(_contextConfig.DefaultProperties);

        public Dictionary<string, object> ObjectCache { get; set; }

        readonly ProjectPathConfig _projectPathConfig = new ProjectPathConfig();
        readonly ContextConfig _contextConfig = new ContextConfig();

        readonly INamedTypeSymbol[] _types;

        public EntityIndexDataProvider() : this(null) { }

        public EntityIndexDataProvider(INamedTypeSymbol[] types) => _types = types;

        public void Configure(Preferences preferences)
        {
            _projectPathConfig.Configure(preferences);
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

            // ReSharper disable once CoVariantArrayConversion
            return entityIndexData.Concat(customEntityIndexData).ToArray();
        }

        IEnumerable<EntityIndexData> CreateEntityIndexData(INamedTypeSymbol type, ISymbol[] members)
        {
            var indexes = members
                .Where(member => member.GetAttribute<AbstractEntityIndexAttribute>(true) != null)
                .ToArray();

            return indexes
                .Select(member =>
                {
                    var componentType = type.ToCompilableString();
                    var contexts = ComponentDataProvider.GetContexts(type, _contextConfig.Contexts);
                    if (contexts.Length == 0)
                        contexts = new[] {_contextConfig.Contexts[0]};

                    return new EntityIndexData(
                        type: GetEntityIndexType(member.GetAttribute<AbstractEntityIndexAttribute>(true)),
                        name: componentType.ShortTypeName().RemoveComponentSuffix(),
                        isCustom: false,
                        customMethods: null,
                        keyType: member.PublicMemberType().ToCompilableString(),
                        componentType: componentType,
                        memberName: member.Name,
                        hasMultiple: indexes.Length > 1,
                        contexts: contexts
                    );
                });
        }

        EntityIndexData CreateCustomEntityIndexData(INamedTypeSymbol type)
        {
            var indexType = type.ToCompilableString();
            return new EntityIndexData(
                indexType,
                indexType.ShortTypeName(),
                true,
                type
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
                    .ToArray(),
                null,
                null,
                null,
                false,
                new[]
                {
                    ((INamedTypeSymbol)type.GetAttribute<CustomEntityIndexAttribute>().ConstructorArguments.First().Value)
                    .ToCompilableString().ShortTypeName().RemoveContextSuffix()
                }
            );
        }

        string GetEntityIndexType(AttributeData attribute)
        {
            var entityIndexType = attribute.ToString();
            if (entityIndexType == typeof(EntityIndexAttribute).FullName)
                return "Entitas.EntityIndex";
            if (entityIndexType == typeof(PrimaryEntityIndexAttribute).FullName)
                return "Entitas.PrimaryEntityIndex";

            throw new Exception($"Unhandled EntityIndexType: {entityIndexType}");
        }
    }
}
