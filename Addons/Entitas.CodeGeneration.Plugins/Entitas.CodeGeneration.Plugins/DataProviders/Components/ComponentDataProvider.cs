using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.CodeGeneration.CodeGenerator;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentDataProvider : IDataProvider, IConfigurable, ICachable, IDoctor {

        public string name { get { return "Component"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties {
            get {
                var dataProviderProperties = _dataProviders
                    .OfType<IConfigurable>()
                    .Select(i => i.defaultProperties)
                    .ToArray();

                return _assembliesConfig.defaultProperties
                    .Merge(_contextsComponentDataProvider.defaultProperties)
                    .Merge(_ignoreNamespacesConfig.defaultProperties)
                    .Merge(dataProviderProperties);
            }
        }

        public Dictionary<string, object> objectCache { get; set; }

        readonly CodeGeneratorConfig _codeGeneratorConfig = new CodeGeneratorConfig();
        readonly AssembliesConfig _assembliesConfig = new AssembliesConfig();
        readonly ContextsComponentDataProvider _contextsComponentDataProvider = new ContextsComponentDataProvider();
        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        static IComponentDataProvider[] getComponentDataProviders() {
            return new IComponentDataProvider[] {
                new ComponentTypeComponentDataProvider(),
                new MemberDataComponentDataProvider(),
                new ContextsComponentDataProvider(),
                new IsUniqueComponentDataProvider(),
                new UniquePrefixComponentDataProvider(),
                new ShouldGenerateComponentComponentDataProvider(),
                new ShouldGenerateMethodsComponentDataProvider(),
                new ShouldGenerateComponentIndexComponentDataProvider(),
                new EventComponentDataProvider()
            };
        }

        readonly Type[] _types;
        readonly IComponentDataProvider[] _dataProviders;

        public ComponentDataProvider() : this(null) {
        }

        public ComponentDataProvider(Type[] types) : this(types, getComponentDataProviders()) {
        }

        protected ComponentDataProvider(Type[] types, IComponentDataProvider[] dataProviders) {
            _types = types;
            _dataProviders = dataProviders;
        }

        public void Configure(Preferences preferences) {
            _codeGeneratorConfig.Configure(preferences);
            _assembliesConfig.Configure(preferences);
            foreach (var dataProvider in _dataProviders.OfType<IConfigurable>()) {
                dataProvider.Configure(preferences);
            }

            _contextsComponentDataProvider.Configure(preferences);
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGeneratorData[] GetData() {
            var types = _types ?? PluginUtil
                            .GetCachedAssemblyResolver(objectCache, _assembliesConfig.assemblies, _codeGeneratorConfig.searchPaths)
                            .GetTypes();

            var dataFromComponents = types
                .Where(type => type.ImplementsInterface<IComponent>())
                .Where(type => !type.IsAbstract)
                .Select(createDataForComponent)
                .ToArray();

            var dataFromNonComponents = types
                .Where(type => !type.ImplementsInterface<IComponent>())
                .Where(type => !type.IsGenericType)
                .Where(hasContexts)
                .SelectMany(createDataForNonComponent)
                .ToArray();

            var dataFromEvents = dataFromComponents
                .Where(data => data.IsEvent())
                .SelectMany(createDataForEvents)
                .ToArray();

            return merge(dataFromEvents, merge(dataFromNonComponents, dataFromComponents));
        }

        ComponentData[] merge(ComponentData[] prioData, ComponentData[] redundantData) {
            var lookup = prioData.ToLookup(data => data.GetTypeName());
            return redundantData
                .Where(data => !lookup.Contains(data.GetTypeName()))
                .Concat(prioData)
                .ToArray();
        }

        ComponentData createDataForComponent(Type type) {
            var data = new ComponentData();
            foreach (var provider in _dataProviders) {
                provider.Provide(type, data);
            }

            return data;
        }

        ComponentData[] createDataForNonComponent(Type type) {
            return getComponentNames(type)
                .Select(componentName => {
                    var data = createDataForComponent(type);
                    data.SetlTypeName(componentName.AddComponentSuffix());
                    data.SetMemberData(new[] {
                        new MemberData(type.ToCompilableString(), "value")
                    });

                    return data;
                }).ToArray();
        }

        ComponentData[] createDataForEvents(ComponentData data) {
            var dataForEvent = new ComponentData(data);
            dataForEvent.IsEvent(false);
            var componentName = dataForEvent.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
            return dataForEvent.GetContextNames()
                .Select(contextName => {
                    var listenerComponentName = contextName + componentName + "Listener";
                    dataForEvent.SetlTypeName(listenerComponentName.AddComponentSuffix());
                    dataForEvent.SetMemberData(new[] {
                        new MemberData("I" + listenerComponentName, "value")
                    });
                    dataForEvent.SetContextNames(new [] { contextName });
                    return dataForEvent;
                })
                .ToArray();
        }

        bool hasContexts(Type type) {
            return _contextsComponentDataProvider.GetContextNames(type).Length != 0;
        }

        string[] getComponentNames(Type type) {
            var attr = Attribute
                .GetCustomAttributes(type)
                .OfType<CustomComponentNameAttribute>()
                .SingleOrDefault();

            if (attr == null) {
                return new[] { type.ToCompilableString().ShortTypeName().AddComponentSuffix() };
            }

            return attr.componentNames;
        }

        public Diagnosis Diagnose() {
            var isStandalone = AppDomain.CurrentDomain
                .GetAllTypes()
                .Any(type => type.FullName == "DesperateDevs.CodeGeneration.CodeGenerator.CLI.Program");

            if (isStandalone) {
                var typeName = typeof(ComponentDataProvider).FullName;
                if (_codeGeneratorConfig.dataProviders.Contains(typeName)) {
                    return new Diagnosis(
                        typeName + " loads and reflects " + string.Join(", ", _assembliesConfig.assemblies) + " and therefore doesn't support server mode!",
                        "Don't use the code generator in server mode with " + typeName,
                        DiagnosisSeverity.Hint
                    );
                }
            }

            return Diagnosis.Healthy;
        }

        public bool Fix() {
            return false;
        }
    }
}
