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

        readonly INamedTypeSymbol[] _types;

        public ComponentDataProvider() : this(null) { }

        public ComponentDataProvider(INamedTypeSymbol[] types) => _types = types;

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

            var dataFromComponents = types
                .Where(type => type.AllInterfaces.Any(i => i.ToCompilableString() == componentInterface))
                .Where(type => !type.IsAbstract)
                .SelectMany(type => CreateDataForComponent(type))
                .ToArray();

            var dataFromNonComponents = types
                .Where(type => !type.AllInterfaces.Any(i => i.ToCompilableString() == componentInterface))
                .Where(type => !type.IsGenericType)
                .Where(type => GetContexts(type, _contextConfig.Contexts).Length != 0)
                .SelectMany(type => CreateDataForNonComponent(type))
                .ToArray();

            var mergedData = Merge(dataFromNonComponents, dataFromComponents);

            var dataFromEvents = mergedData
                .Where(data => data.EventData != null)
                .SelectMany(data => CreateDataForEvents(data))
                .ToArray();

            return Merge(dataFromEvents, mergedData);
        }

        ComponentData[] Merge(ComponentData[] prioData, ComponentData[] redundantData)
        {
            var lookup = prioData.ToLookup(data => data.Type);
            return redundantData
                .Where(data => !lookup.Contains(data.Type))
                .Concat(prioData)
                .ToArray();
        }

        ComponentData[] CreateDataForComponent(INamedTypeSymbol type)
        {
            var contexts = GetContexts(type, _contextConfig.Contexts);
            return contexts.Length == 0
                ? new[] {CreateDataForComponent(type, _contextConfig.Contexts[0])}
                : contexts.Select(context => CreateDataForComponent(type, context)).ToArray();
        }

        ComponentData CreateDataForComponent(INamedTypeSymbol type, string context)
        {
            var componentInterface = typeof(IComponent).ToCompilableString();
            var generatesObject = !type.AllInterfaces.Any(i => i.ToCompilableString() == typeof(IComponent).ToCompilableString());
            var eventAttributes = type.GetAttributes<EventAttribute>();
            return new ComponentData(
                type: type.ToCompilableString(),
                memberData: type.GetPublicMembers(type.AllInterfaces.Any(i => i.ToCompilableString() == componentInterface))
                    .Select(member => new MemberData(member.PublicMemberType().ToCompilableString(), member.Name))
                    .ToArray(),
                context: context,
                isUnique: type.GetAttribute<UniqueAttribute>() != null,
                flagPrefix: (string)(type.GetAttribute<FlagPrefixAttribute>()?.ConstructorArguments.First().Value ?? "Is"),
                generates: type.GetAttribute<DontGenerateAttribute>() == null,
                generatesIndex: (bool)(type.GetAttribute<DontGenerateAttribute>()?.ConstructorArguments.First().Value ?? true),
                generatesObject: generatesObject,
                objectType: generatesObject ? type.ToCompilableString() : null,
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

        IEnumerable<ComponentData> CreateDataForNonComponent(INamedTypeSymbol type)
        {
            var componentNames = type.GetAttribute<ComponentNamesAttribute>()?.ConstructorArguments.First().Values.Select(arg => (string)arg.Value).ToArray()
                                 ?? new[] {type.ToCompilableString().ShortTypeName().AddComponentSuffix()};
            return componentNames.SelectMany(componentName =>
            {
                return CreateDataForComponent(type)
                    .Select(data =>
                    {
                        data.Type = componentName.AddComponentSuffix();
                        data.MemberData = new[] {new MemberData(type.ToCompilableString(), "Value")};
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
                EventData = null
            };
        });

        public static string[] GetContexts(INamedTypeSymbol type, string[] predefinedContexts)
        {
            var contexts = new List<string>();
            var contextAttribute = typeof(ContextAttribute).ToCompilableString();
            foreach (var attribute in type.GetAttributes())
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
                    var name = (string)attribute.ConstructorArguments.First().Value;
                    contexts.Add(name);
                }
            }

            return contexts.ToArray();
        }
    }
}
