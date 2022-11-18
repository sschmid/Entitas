using System.Collections.Generic;
using System.Linq;
using Jenny;
using Jenny.Plugins;
using DesperateDevs.Extensions;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.Plugins.Attributes;
using Entitas.CodeGeneration.Plugins;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Entitas.Roslyn.CodeGeneration.Plugins
{
    public class ComponentDataProvider : IDataProvider, IConfigurable, ICachable
    {
        public string Name => "Component (Roslyn)";
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

        public ComponentDataProvider() : this(null) { }

        public ComponentDataProvider(INamedTypeSymbol[] types) => _types = types;

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

            var dataFromComponents = types
                .Where(type => type.AllInterfaces.Any(i => i.ToCompilableString() == componentInterface))
                .Where(type => !type.IsAbstract)
                .Select(type => CreateDataForComponent(type))
                .ToArray();

            var dataFromNonComponents = types
                .Where(type => !type.AllInterfaces.Any(i => i.ToCompilableString() == componentInterface))
                .Where(type => !type.IsGenericType)
                .Where(type => GetContexts(type, _contextConfig.Contexts).Length != 0)
                .SelectMany(type => CreateDataForNonComponent(type))
                .ToArray();

            var mergedData = Merge(dataFromNonComponents, dataFromComponents);

            var dataFromEvents = mergedData
                .Where(data => data.IsEvent)
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

        ComponentData CreateDataForComponent(INamedTypeSymbol type)
        {
            var componentInterface = typeof(IComponent).ToCompilableString();
            var isComponent = type.AllInterfaces.Any(i => i.ToCompilableString() == componentInterface);

            var data = new ComponentData();
            data.Type = type.ToCompilableString();
            data.MemberData = type.GetPublicMembers(isComponent)
                .Select(member => new MemberData(member.PublicMemberType().ToCompilableString(), member.Name))
                .ToArray();
            var contexts = GetContexts(type, _contextConfig.Contexts);
            if (contexts.Length == 0)
                contexts = new[] {_contextConfig.Contexts[0]};
            data.Contexts = contexts;
            data.IsUnique = type.GetAttribute<UniqueAttribute>() != null;
            data.FlagPrefix = (string)(type.GetAttribute<FlagPrefixAttribute>()?.ConstructorArguments.First().Value ?? "is");
            data.Generates = type.GetAttribute<DontGenerateAttribute>() == null;
            var generatesObject = !type.AllInterfaces.Any(i => i.ToCompilableString() == typeof(IComponent).ToCompilableString());
            data.GeneratesObject = generatesObject;
            data.ObjectType = generatesObject ? type.ToCompilableString() : null;
            data.GeneratesIndex = (bool)(type.GetAttribute<DontGenerateAttribute>()?.ConstructorArguments.First().Value ?? true);
            var eventAttributes = type.GetAttributes<EventAttribute>();
            data.IsEvent = eventAttributes.Length > 0;
            data.EventData = eventAttributes.Length > 0
                ? eventAttributes.Select(attr => new EventData((EventTarget)attr.ConstructorArguments[0].Value, (EventType)attr.ConstructorArguments[1].Value, (int)attr.ConstructorArguments[2].Value)).ToArray()
                : null;

            return data;
        }

        ComponentData[] CreateDataForNonComponent(INamedTypeSymbol type)
        {
            var componentNames = type.GetAttribute<ComponentNamesAttribute>()?.ConstructorArguments.First().Values.Select(arg => (string)arg.Value).ToArray()
                                 ?? new[] {type.ToCompilableString().ShortTypeName().AddComponentSuffix()};
            return componentNames.Select(componentName =>
            {
                var data = CreateDataForComponent(type);
                data.Type = componentName.AddComponentSuffix();
                data.MemberData = new[] {new MemberData(type.ToCompilableString(), "value")};
                return data;
            }).ToArray();
        }

        ComponentData[] CreateDataForEvents(ComponentData data) => data.Contexts.SelectMany(context =>
            data.EventData.Select(eventData =>
            {
                var dataForEvent = new ComponentData(data);
                var optionalContext = dataForEvent.Contexts.Length > 1 ? context : string.Empty;
                var listener = optionalContext + data.EventComponentName(eventData) + eventData.GetEventTypeSuffix().AddListenerSuffix();
                dataForEvent.Type = listener.AddComponentSuffix();
                dataForEvent.MemberData = new[] {new MemberData($"System.Collections.Generic.List<I{listener}>", "value")};
                dataForEvent.Contexts = new[] {context};
                dataForEvent.IsUnique = false;
                dataForEvent.GeneratesObject = false;
                dataForEvent.IsEvent = false;
                dataForEvent.EventData = null;
                return dataForEvent;
            })).ToArray();

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
                    // Entitas.CodeGeneration.Attributes.ContextAttribute
                    var name = (string)attribute.ConstructorArguments.First().Value;
                    contexts.Add(name);
                }
            }

            return contexts.ToArray();
        }
    }
}
