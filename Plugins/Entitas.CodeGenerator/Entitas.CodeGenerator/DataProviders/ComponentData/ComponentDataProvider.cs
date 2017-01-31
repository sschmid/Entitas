using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.CodeGenerator.Api;

namespace Entitas.CodeGenerator {

    public class ComponentDataProvider : ICodeGeneratorDataProvider {

        readonly Type[] _types;
        readonly IComponentDataProvider[] _dataProviders;

        public ComponentDataProvider(Type[] types) {
            _types = types;

            _dataProviders = new IComponentDataProvider[] {
                new ComponentTypeComponentDataProvider(),
                new ComponentNameComponentDataProvider(),
                new MemberInfosComponentDataProvider(),
                new ContextsComponentDataProvider(),
                new IsUniqueComponentDataProvider(),
                new UniquePrefixComponentDataProvider(),
                new ShouldGenerateComponentComponentDataProvider(),
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
                    data.SetMemberInfos(new List<PublicMemberInfo> {
                        new PublicMemberInfo(type, "value")
                    });

                    return data;
                })
                .ToArray();
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
