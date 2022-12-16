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

        readonly INamedTypeSymbol[] _symbols;

        readonly string _componentInterface = typeof(IComponent).ToCompilableString();

        public EntityIndexDataProvider() : this(null) { }

        public EntityIndexDataProvider(INamedTypeSymbol[] symbols) => _symbols = symbols;

        public void Configure(Preferences preferences)
        {
            _projectPathConfig.Configure(preferences);
            _contextConfig.Configure(preferences);
        }

        ProjectParser GetProjectParser()
        {
            var key = typeof(ProjectParser).FullName;
            if (!ObjectCache.TryGetValue(key, out var projectParser))
            {
                projectParser = new ProjectParser(_projectPathConfig.ProjectPath);
                ObjectCache.Add(key, projectParser);
            }

            return (ProjectParser)projectParser;
        }

        public CodeGeneratorData[] GetData()
        {
            var symbols = _symbols ?? GetProjectParser().GetTypes();
            var concrete = symbols
                .Where(symbol => !symbol.IsAbstract)
                .ToArray();

            var entityIndexData = concrete
                .Where(symbol => symbol.AllInterfaces.Any(i => i.ToCompilableString() == _componentInterface))
                .ToDictionary(
                    symbol => symbol,
                    symbol => symbol.GetPublicMembers(true))
                .Where(kvp => kvp.Value.Any(symbol => symbol.GetAttribute<AbstractEntityIndexAttribute>(true) != null))
                .SelectMany(kvp => CreateEntityIndexData(kvp.Key, kvp.Value));

            var customEntityIndexData = concrete
                .Where(symbol => symbol.GetAttribute<CustomEntityIndexAttribute>() != null)
                .Select(symbol => CreateCustomEntityIndexData(symbol));

            // ReSharper disable once CoVariantArrayConversion
            return entityIndexData.Concat(customEntityIndexData).ToArray();
        }

        IEnumerable<EntityIndexData> CreateEntityIndexData(INamedTypeSymbol symbol, ISymbol[] members) => members
            .Select(member =>
            {
                var componentType = symbol.ToCompilableString();
                var contexts = ComponentDataProvider.GetContexts(symbol, _contextConfig.Contexts);
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
                    hasMultiple: members.Length > 1,
                    contexts: contexts
                );
            });

        EntityIndexData CreateCustomEntityIndexData(INamedTypeSymbol symbols)
        {
            var indexType = symbols.ToCompilableString();
            return new EntityIndexData(
                indexType,
                indexType.ShortTypeName(),
                true,
                symbols
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
                    ((INamedTypeSymbol)symbols.GetAttribute<CustomEntityIndexAttribute>().ConstructorArguments.First().Value)
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
