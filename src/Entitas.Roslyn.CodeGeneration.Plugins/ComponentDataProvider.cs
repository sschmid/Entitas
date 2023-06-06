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
    public class ComponentDataProvider : IDataProvider, IConfigurable, ICachable
    {
        public string Name => "Component (Roslyn)";
        public int Order => 0;
        public bool RunInDryMode => true;

        public Dictionary<string, string> DefaultProperties
        {
            get
            {
                var dataProviderProperties = _dataProviders
                    .OfType<IConfigurable>()
                    .Select(i => i.DefaultProperties)
                    .ToArray();

                return _projectPathConfig.DefaultProperties
                    .Merge(_contextsComponentDataProvider.DefaultProperties)
                    .Merge(_ignoreNamespacesConfig.DefaultProperties)
                    .Merge(dataProviderProperties);
            }
        }

        public Dictionary<string, object> ObjectCache { get; set; }

        readonly ProjectPathConfig _projectPathConfig = new ProjectPathConfig();
        readonly ContextsComponentDataProvider _contextsComponentDataProvider;
        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        static IComponentDataProvider[] getComponentDataProviders()
        {
            return new IComponentDataProvider[]
            {
                new ComponentTypeComponentDataProvider(),
                new MemberDataComponentDataProvider(),
                new ContextsComponentDataProvider(),
                new IsUniqueComponentDataProvider(),
                new FlagPrefixComponentDataProvider(),
                new ShouldGenerateComponentComponentDataProvider(),
                new ShouldGenerateMethodsComponentDataProvider(),
                new ShouldGenerateComponentIndexComponentDataProvider(),
                new EventComponentDataProvider()
            };
        }

        readonly INamedTypeSymbol[] _types;
        readonly IComponentDataProvider[] _dataProviders;

        public ComponentDataProvider() : this(null) { }

        public ComponentDataProvider(INamedTypeSymbol[] types) : this(types, getComponentDataProviders()) { }

        public ComponentDataProvider(INamedTypeSymbol[] types, IComponentDataProvider[] dataProviders)
        {
            _types = types;
            _dataProviders = dataProviders;
            _contextsComponentDataProvider = new ContextsComponentDataProvider();
        }

        public void Configure(Preferences preferences)
        {
            _projectPathConfig.Configure(preferences);
            foreach (var dataProvider in _dataProviders.OfType<IConfigurable>())
                dataProvider.Configure(preferences);

            _contextsComponentDataProvider.Configure(preferences);
            _ignoreNamespacesConfig.Configure(preferences);
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
                .Select(createDataForComponent)
                .ToArray();

            var dataFromNonComponents = types
                .Where(type => !type.AllInterfaces.Any(i => i.ToCompilableString() == componentInterface))
                .Where(type => !type.IsGenericType)
                .Where(symbol => hasContexts(symbol))
                .SelectMany(symbol => createDataForNonComponent(symbol))
                .ToArray();

            var mergedData = merge(dataFromNonComponents, dataFromComponents);

            var dataFromEvents = mergedData
                .Where(data => data.IsEvent())
                .SelectMany(data => createDataForEvents(data))
                .ToArray();

            return merge(dataFromEvents, mergedData);
        }

        ComponentData[] merge(ComponentData[] prioData, ComponentData[] redundantData)
        {
            var lookup = prioData.ToLookup(data => data.GetTypeName());
            return redundantData
                .Where(data => !lookup.Contains(data.GetTypeName()))
                .Concat(prioData)
                .ToArray();
        }

        ComponentData createDataForComponent(INamedTypeSymbol type)
        {
            var data = new ComponentData();
            foreach (var provider in _dataProviders)
                provider.Provide(type, data);

            return data;
        }

        ComponentData[] createDataForNonComponent(INamedTypeSymbol type) => getComponentNames(type)
            .Select(componentName =>
            {
                var data = createDataForComponent(type);
                data.SetTypeName(componentName.AddComponentSuffix());
                data.SetMemberData(new[]
                {
                    new MemberData(type.ToCompilableString(), "value")
                });

                return data;
            }).ToArray();

        ComponentData[] createDataForEvents(ComponentData data) => data.GetContextNames()
            .SelectMany(contextName =>
                data.GetEventData().Select(eventData =>
                {
                    var dataForEvent = new ComponentData(data);
                    dataForEvent.IsEvent(false);
                    dataForEvent.IsUnique(false);
                    dataForEvent.ShouldGenerateComponent(false);
                    var eventComponentName = data.EventComponentName(eventData);
                    var eventTypeSuffix = eventData.GetEventTypeSuffix();
                    var optionalContextName = dataForEvent.GetContextNames().Length > 1 ? contextName : string.Empty;
                    var listenerComponentName = optionalContextName + eventComponentName + eventTypeSuffix.AddListenerSuffix();
                    dataForEvent.SetTypeName(listenerComponentName.AddComponentSuffix());
                    dataForEvent.SetMemberData(new[]
                    {
                        new MemberData($"System.Collections.Generic.List<I{listenerComponentName}>", "value")
                    });
                    dataForEvent.SetContextNames(new[] {contextName});
                    return dataForEvent;
                }).ToArray()
            ).ToArray();

        bool hasContexts(INamedTypeSymbol type) => _contextsComponentDataProvider.GetContextNames(type).Length != 0;

        string[] getComponentNames(INamedTypeSymbol type)
        {
            var attr = type.GetAttribute<ComponentNameAttribute>();
            if (attr == null)
                return new[] {type.ToCompilableString().TypeName().AddComponentSuffix()};

            return attr.ConstructorArguments.First().Values.Select(arg => (string)arg.Value).ToArray();
        }
    }
}
