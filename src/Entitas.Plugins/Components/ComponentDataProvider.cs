using System.Collections.Generic;
using System.Linq;
using Jenny;
using Jenny.Plugins;
using DesperateDevs.Extensions;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.Plugins.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Entitas.Plugins
{
    public class ComponentDataProvider : IDataProvider, IConfigurable, ICachable
    {
        public string Name => "Component (Roslyn)";
        public int Order => 0;
        public bool RunInDryMode => true;

        public Dictionary<string, string> DefaultProperties =>
            _projectPathConfig.DefaultProperties.Merge(_contextConfig.DefaultProperties);

        public Dictionary<string, object> ObjectCache { get; set; }

        readonly ProjectPathConfig _projectPathConfig = new ProjectPathConfig();
        readonly ContextConfig _contextConfig = new ContextConfig();

        readonly INamedTypeSymbol[] _symbols;

        readonly string _componentInterface = typeof(IComponent).ToCompilableString();

        public ComponentDataProvider() : this(null) { }

        public ComponentDataProvider(INamedTypeSymbol[] symbols) => _symbols = symbols;

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
            var lookup = symbols.ToLookup(symbol => symbol.AllInterfaces.Any(i => i.ToCompilableString() == _componentInterface));
            var components = lookup[true];
            var nonComponents = lookup[false];

            var dataFromComponents = components
                .Where(symbol => !symbol.IsAbstract)
                .SelectMany(symbol => CreateData(symbol, true))
                .ToArray();

            var dataFromNonComponents = nonComponents
                .Where(symbol => !symbol.IsGenericType)
                .Where(symbol => GetContexts(symbol, _contextConfig.Contexts).Length != 0)
                .SelectMany(symbol => CreateDataForNonComponent(symbol))
                .ToArray();

            var mergedData = dataFromNonComponents
                .UnionBy(dataFromComponents, data => data.Type)
                .ToArray();

            var dataFromEvents = mergedData
                .Where(data => data.EventData != null)
                .SelectMany(data => CreateDataForEvents(data))
                .ToArray();

            // ReSharper disable once CoVariantArrayConversion
            return dataFromEvents
                .UnionBy(mergedData, data => data.Type)
                .ToArray();
        }

        IEnumerable<ComponentData> CreateData(INamedTypeSymbol symbol, bool isComponent)
        {
            var contexts = GetContexts(symbol, _contextConfig.Contexts);
            return contexts.Length == 0
                ? new[] {CreateData(symbol, _contextConfig.Contexts[0], isComponent)}
                : contexts.Select(context => CreateData(symbol, context, isComponent));
        }

        ComponentData CreateData(INamedTypeSymbol symbol, string context, bool isComponent)
        {
            var eventAttributes = symbol.GetAttributes<EventAttribute>();
            return new ComponentData(
                type: symbol.ToCompilableString(),
                memberData: symbol.GetPublicMembers(isComponent)
                    .Select(member => new MemberData(member.PublicMemberType().ToCompilableString(), member.Name))
                    .ToArray(),
                context: context,
                isUnique: symbol.GetAttribute<UniqueAttribute>() != null,
                flagPrefix: (string)(symbol.GetAttribute<FlagPrefixAttribute>()?.ConstructorArguments.First().Value ?? "Is"),
                generates: symbol.GetAttribute<DontGenerateAttribute>() == null,
                generatesIndex: (bool)(symbol.GetAttribute<DontGenerateAttribute>()?.ConstructorArguments.First().Value ?? true),
                generatesObject: !isComponent,
                objectType: isComponent ? null : symbol.ToCompilableString(),
                eventData: eventAttributes.Length > 0
                    ? eventAttributes
                        .Select(attr =>
                        {
                            var args = attr.ConstructorArguments;
                            return new EventData(
                                (EventTarget)args[0].Value,
                                (EventType)args[1].Value,
                                (int)args[2].Value);
                        })
                        .ToArray()
                    : null
            );
        }

        IEnumerable<ComponentData> CreateDataForNonComponent(INamedTypeSymbol symbol)
        {
            var componentNames = symbol.GetAttribute<ComponentNamesAttribute>()?.ConstructorArguments.First().Values.Select(arg => (string)arg.Value)
                                 ?? new[] {symbol.ToCompilableString().ShortTypeName()};
            return componentNames.SelectMany(componentName =>
            {
                return CreateData(symbol, false)
                    .Select(data =>
                    {
                        data.Type = componentName.AddComponentSuffix();
                        data.MemberData = new[] {new MemberData(symbol.ToCompilableString(), "Value")};
                        return data;
                    });
            });
        }

        IEnumerable<ComponentData> CreateDataForEvents(ComponentData data) => data.EventData.Select(eventData =>
        {
            var listener = data.Context + data.EventComponentName(eventData) + eventData.GetEventTypeSuffix().AddListenerSuffix();
            return new ComponentData(data)
            {
                Type = listener.AddComponentSuffix(),
                MemberData = new[] {new MemberData($"System.Collections.Generic.List<I{listener}>", "Value")},
                IsUnique = false,
                GeneratesObject = false,
                ObjectType = null,
                EventData = null
            };
        });

        public static string[] GetContexts(INamedTypeSymbol symbol, string[] predefinedContexts)
        {
            var contexts = new List<string>();
            var contextAttribute = typeof(ContextAttribute).ToCompilableString();
            foreach (var attribute in symbol.GetAttributes())
            {
                var contextCandidate = attribute.AttributeClass.ToString().Replace("Attribute", string.Empty);
                if (attribute.AttributeClass.BaseType == null && predefinedContexts.Contains(contextCandidate))
                {
                    // Possible compiler error. Just take the attribute name
                    contexts.Add(contextCandidate);
                }
                else if (attribute.AttributeClass.BaseType != null && attribute.AttributeClass.BaseType.ToCompilableString() == contextAttribute)
                {
                    // Generated context attribute
                    var declaration = attribute.AttributeConstructor.DeclaringSyntaxReferences.First().GetSyntax();
                    var baseConstructorInit = (ConstructorInitializerSyntax)declaration.DescendantNodes().First(node => node.IsKind(SyntaxKind.BaseConstructorInitializer));
                    var argument = (LiteralExpressionSyntax)baseConstructorInit.ArgumentList.Arguments.First().Expression;
                    var name = argument.ToString().Replace("\"", string.Empty);
                    contexts.Add(name);
                }
                else if (attribute.AttributeClass.ToCompilableString().Contains(contextAttribute))
                {
                    // Entitas.Plugins.Attributes.ContextAttribute
                    contexts.Add((string)attribute.ConstructorArguments.First().Value);
                }
            }

            return contexts.ToArray();
        }
    }
}
