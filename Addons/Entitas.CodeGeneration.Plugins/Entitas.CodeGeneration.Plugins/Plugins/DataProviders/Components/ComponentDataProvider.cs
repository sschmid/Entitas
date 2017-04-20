using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.CodeGeneration.Attributes;
using Entitas.CodeGeneration.CodeGenerator;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentDataProvider : ICodeGeneratorDataProvider, IConfigurable {

        public string name { get { return "Component"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties {
            get {
                var dataProviderProperties = _dataProviders
                    .OfType<IConfigurable>()
                    .Select(i => i.defaultProperties)
                    .ToArray();

                return _assembliesConfig
                    .defaultProperties
                    .Merge(dataProviderProperties);
            }
        }

        readonly CodeGeneratorConfig _codeGeneratorConfig = new CodeGeneratorConfig();
        readonly AssembliesConfig _assembliesConfig = new AssembliesConfig();

        static IComponentDataProvider[] getComponentDataProviders() {
            return new IComponentDataProvider[] {
                new ComponentTypeComponentDataProvider(),
                new MemberDataComponentDataProvider(),
                new ContextsComponentDataProvider(),
                new IsUniqueComponentDataProvider(),
                new UniquePrefixComponentDataProvider(),
                new ShouldGenerateComponentComponentDataProvider(),
                new ShouldGenerateMethodsComponentDataProvider(),
                new ShouldGenerateComponentIndexComponentDataProvider()
            };
        }

        Type[] _types;
        IComponentDataProvider[] _dataProviders;

        public ComponentDataProvider() : this(null) {
        }

        public ComponentDataProvider(Type[] types) : this(types, getComponentDataProviders()) {
        }

        protected ComponentDataProvider(Type[] types, IComponentDataProvider[] dataProviders) {
            _types = types;
            _dataProviders = dataProviders;
        }

        public void Configure(Properties properties) {
            _codeGeneratorConfig.Configure(properties);
            _assembliesConfig.Configure(properties);
            foreach (var dataProvider in _dataProviders.OfType<IConfigurable>()) {
                dataProvider.Configure(properties);
            }
        }

        public CodeGeneratorData[] GetData() {
            if (_types == null) {
                _types = PluginUtil
                    .GetAssembliesResolver(_assembliesConfig.assemblies, _codeGeneratorConfig.searchPaths)
                    .GetTypes();
            }

            var dataFromComponents = _types
                .Where(type => type.ImplementsInterface<IComponent>())
                .Where(type => !type.IsAbstract)
                .Select(type => createDataForComponent(type));

            var dataFromNonComponents = _types
                .Where(type => !type.ImplementsInterface<IComponent>())
                .Where(type => !type.IsGenericType)
                .Where(type => hasContexts(type))
                .SelectMany(type => createDataForNonComponent(type));

            var generatedComponentsLookup = dataFromNonComponents.ToLookup(data => data.GetFullTypeName());
            return dataFromComponents
                .Where(data => !generatedComponentsLookup.Contains(data.GetFullTypeName()))
                .Concat(dataFromNonComponents)
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
                    data.SetFullTypeName(componentName.AddComponentSuffix());
                    data.SetMemberData(new [] {
                        new MemberData(type.ToCompilableString(), "value")
                    });

                    return data;
                }).ToArray();
        }

        bool hasContexts(Type type) {
            return ContextsComponentDataProvider.GetContextNames(type).Length != 0;
        }

        string[] getComponentNames(Type type) {
            var attr = Attribute
                .GetCustomAttributes(type)
                .OfType<CustomComponentNameAttribute>()
                .SingleOrDefault();

            if (attr == null) {
                return new [] { type.ToCompilableString().ShortTypeName().AddComponentSuffix() };
            }

            return attr.componentNames;
        }
    }
}
