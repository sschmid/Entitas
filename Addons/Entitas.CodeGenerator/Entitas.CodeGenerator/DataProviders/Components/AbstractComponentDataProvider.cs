using System;
using System.Linq;
using Entitas.CodeGenerator.Api;

namespace Entitas.CodeGenerator {

    public abstract class AbstractComponentDataProvider : ICodeGeneratorDataProvider {

        public abstract string name { get; }
        public abstract bool isEnabledByDefault { get; }

        readonly Type[] _types;
        readonly IComponentDataProvider[] _dataProviders;

        protected AbstractComponentDataProvider(IComponentDataProvider[] dataProviders, Type[] types) {
            _types = types;
            _dataProviders = dataProviders;
        }

        public CodeGeneratorData[] GetData() {
            var dataFromComponents = _types
                .Where(type => type.ImplementsInterface<IComponent>())
                .Where(type => !type.IsAbstract)
                .Select(type => createDataForComponent(type));

            var dataFromNonComponents = _types
                .Where(type => !type.ImplementsInterface<IComponent>())
                .Where(type => !type.IsGenericType)
                .Where(type => hasContexts(type))
                .SelectMany(type => createDataForNonComponent(type));

            var generatedComponentsLookup = dataFromNonComponents.ToLookup(data => data.GetFullComponentName());
            return dataFromComponents
                .Where(data => !generatedComponentsLookup.Contains(data.GetFullComponentName()))
                .Concat(dataFromNonComponents)
                .ToArray();
        }

        ComponentData createDataForComponent(Type type) {
            var data = new ComponentData();
            foreach(var provider in _dataProviders) {
                provider.Provide(type, data);
            }

            return data;
        }

        ComponentData[] createDataForNonComponent(Type type) {
            return getComponentNames(type)
                .Select(componentName => {
                var data = createDataForComponent(type);
                data.SetFullTypeName(componentName.AddComponentSuffix());
                data.SetFullComponentName(componentName.AddComponentSuffix());
                data.SetComponentName(componentName.RemoveComponentSuffix());
                data.SetMemberData(new [] {
                    new MemberData(type.ToCompilableString(), "value")
                });

                return data;
            })
                .ToArray();
        }

        bool hasContexts(Type type) {
            return ContextsComponentDataProvider.GetContextNames(type).Length != 0;
        }

        string[] getComponentNames(Type type) {
            var attr = Attribute
                .GetCustomAttributes(type)
                .OfType<CustomComponentNameAttribute>()
                .SingleOrDefault();

            if(attr == null) {
                return new[] { type.ToCompilableString().ShortTypeName().AddComponentSuffix() };
            }

            return attr.componentNames;
        }
    }
}
