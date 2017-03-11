using System;

namespace Entitas.CodeGenerator {

    public class ComponentNameComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            var componentName = type.ToCompilableString().RemoveDots();
            data.SetFullComponentName(componentName.AddComponentSuffix());
            data.SetComponentName(componentName.RemoveComponentSuffix());
        }
    }

    public static class ComponentNameComponentDataExtension {

        public const string COMPONENT_FULL_COMPONENT_NAME = "component_fullComponentName";
        public const string COMPONENT_COMPONENT_NAME = "component_componentName";

        public static string GetFullComponentName(this ComponentData data) {
            return (string)data[COMPONENT_FULL_COMPONENT_NAME];
        }

        public static void SetFullComponentName(this ComponentData data, string componentName) {
            data[COMPONENT_FULL_COMPONENT_NAME] = componentName;
        }

        public static string GetComponentName(this ComponentData data) {
            return (string)data[COMPONENT_COMPONENT_NAME];
        }

        public static void SetComponentName(this ComponentData data, string componentName) {
            data[COMPONENT_COMPONENT_NAME] = componentName;
        }
    }
}
