using System;
using System.Collections.Generic;
using System.Linq;
using Jenny;
using Jenny.Generator;
using DesperateDevs.Extensions;
using DesperateDevs.Reflection;
using DesperateDevs.Serialization;
using Entitas.Plugins.Attributes;

namespace Entitas.Plugins
{
    public class ComponentDataProvider : IDataProvider, IConfigurable, ICachable, IDoctor
    {
        public string Name => "Component";
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

        public ComponentDataProvider() : this(null) { }

        public ComponentDataProvider(Type[] types) => _types = types;

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
                .GetTypes();

            var dataFromComponents = types
                .Where(type => type.ImplementsInterface<IComponent>())
                .Where(type => !type.IsAbstract)
                .Select(type => CreateDataForComponent(type))
                .ToArray();

            var dataFromNonComponents = types
                .Where(type => !type.ImplementsInterface<IComponent>())
                .Where(type => !type.IsGenericType)
                .Where(type => Attribute.GetCustomAttributes(type).OfType<ContextAttribute>().Any())
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

        ComponentData CreateDataForComponent(Type type)
        {
            var attributes = Attribute.GetCustomAttributes(type);
            var data = new ComponentData();
            data.Type = type.ToCompilableString();
            data.MemberData = type.GetPublicMemberInfos()
                .Select(info => new MemberData(info.Type.ToCompilableString(), info.Name))
                .ToArray();
            var contexts = attributes.OfType<ContextAttribute>().Select(attr => attr.Name).ToArray();
            if (contexts.Length == 0)
                contexts = new[] {_contextConfig.Contexts[0]};
            data.Contexts = contexts;
            data.IsUnique = attributes.OfType<UniqueAttribute>().Any();
            data.FlagPrefix = attributes.OfType<FlagPrefixAttribute>().SingleOrDefault()?.Prefix ?? "is";
            data.Generates = !attributes.OfType<DontGenerateAttribute>().Any();
            var generatesObject = !type.ImplementsInterface<IComponent>();
            data.GeneratesObject = generatesObject;
            data.ObjectType = generatesObject ? type.ToCompilableString() : null;
            data.GeneratesIndex = attributes.OfType<DontGenerateAttribute>().SingleOrDefault()?.GenerateIndex ?? true;
            var eventAttributes = attributes.OfType<EventAttribute>().ToArray();
            data.IsEvent = eventAttributes.Length > 0;
            data.EventData = eventAttributes.Length > 0
                ? eventAttributes.Select(attr => new EventData(attr.EventTarget, attr.EventType, attr.Order)).ToArray()
                : null;

            return data;
        }

        ComponentData[] CreateDataForNonComponent(Type type)
        {
            var componentNames = ((ComponentNamesAttribute)Attribute.GetCustomAttribute(type, typeof(ComponentNamesAttribute)))?.ComponentNames
                                 ?? new[] {type.ToCompilableString().ShortTypeName().AddComponentSuffix()};
            return componentNames.Select(componentName =>
            {
                var data = CreateDataForComponent(type);
                data.Type = componentName.AddComponentSuffix();
                data.MemberData = new[] {new MemberData(type.ToCompilableString(), "Value")};
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
                dataForEvent.MemberData = new[] {new MemberData($"System.Collections.Generic.List<I{listener}>", "Value")};
                dataForEvent.Contexts = new[] {context};
                dataForEvent.IsUnique = false;
                dataForEvent.GeneratesObject = false;
                dataForEvent.IsEvent = false;
                dataForEvent.EventData = null;
                return dataForEvent;
            })).ToArray();

        public Diagnosis Diagnose()
        {
            var isStandalone = AppDomain.CurrentDomain
                .GetAllTypes()
                .Any(type => type.FullName.StartsWith("Jenny.Generator.Cli"));

            if (isStandalone)
            {
                var typeName = typeof(ComponentDataProvider).FullName;
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
