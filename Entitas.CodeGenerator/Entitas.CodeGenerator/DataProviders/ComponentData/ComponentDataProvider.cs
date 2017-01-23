using System;
using System.Linq;
using Entitas.Api;
using Entitas.CodeGenerator.Api;
using Entitas.Utils;

namespace Entitas.CodeGenerator {

    public class ComponentDataProvider : ICodeGeneratorDataProvider {

        readonly Type[] _types;
        readonly IComponentDataProvider[] _componentDataProviders;
        readonly IComponentDataProvider[] _nonComponentDataProviders;

        public ComponentDataProvider(Type[] types) {
            _types = types;

            _componentDataProviders = new IComponentDataProvider[] {
                new FullTypeNameComponentDataProvider(),
                new MemberInfosComponentDataProvider(),
                new ContextsComponentDataProvider(),
                new IsUniqueComponentDataProvider(),
                new UniquePrefixComponentDataProvider(),
                new ShouldGenerateComponentComponentDataProvider(false),
                new ShouldGenerateMethodsComponentDataProvider(),
                new ShouldGenerateComponentIndexComponentDataProvider(),
                new ShouldHideInBlueprintInspectorComponentDataProvider()
            };

            _nonComponentDataProviders = new IComponentDataProvider[] {
                new FullTypeNameComponentDataProvider(),
                new MemberInfosComponentDataProvider(),
                new ContextsComponentDataProvider(),
                new IsUniqueComponentDataProvider(),
                new UniquePrefixComponentDataProvider(),
                new ShouldGenerateComponentComponentDataProvider(true),
                new ShouldGenerateMethodsComponentDataProvider(),
                new ShouldGenerateComponentIndexComponentDataProvider(),
                new ShouldHideInBlueprintInspectorComponentDataProvider()
            };
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

            var generatedComponentsLookup = dataFromNonComponents.ToLookup(data => data.GetFullTypeName());
            return dataFromComponents
                .Where(data => !generatedComponentsLookup.Contains(data.GetFullTypeName()))
                .Concat(dataFromNonComponents)
                .ToArray();
        }

        ComponentData createDataForComponent(Type type) {
            var data = new ComponentData();
            foreach(var provider in _componentDataProviders) {
                provider.Provide(type, data);
            }

            return data;
        }

        ComponentData[] createDataForNonComponent(Type type) {
            return getComponentNames(type).Select(componentName => {
                var data = new ComponentData();
                foreach(var provider in _nonComponentDataProviders) {
                    provider.Provide(type, data);
                }

                return data;
            }).ToArray();
        }

        bool hasContexts(Type type) {
            return Attribute
                .GetCustomAttributes(type)
                .OfType<ContextAttribute>()
                .Any();
        }

        string[] getComponentNames(Type type) {
            var attr = Attribute
                .GetCustomAttributes(type)
                .OfType<CustomComponentNameAttribute>()
                .SingleOrDefault();

            if(attr == null) {
                var nameSplit = type.ToCompilableString().Split('.');
                var componentName = nameSplit[nameSplit.Length - 1].AddComponentSuffix();
                return new[] { componentName };
            }

            return attr.componentNames;
        }
    }
}
