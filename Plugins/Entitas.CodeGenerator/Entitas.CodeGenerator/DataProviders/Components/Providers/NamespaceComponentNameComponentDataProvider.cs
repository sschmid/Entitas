using System;

namespace Entitas.CodeGenerator {

    public class NamespaceComponentNameComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            var componentName = type.ToCompilableString().Replace(".", string.Empty);
            data.SetFullComponentName(componentName.AddComponentSuffix());
            data.SetComponentName(componentName.RemoveComponentSuffix());
        }
    }
}
