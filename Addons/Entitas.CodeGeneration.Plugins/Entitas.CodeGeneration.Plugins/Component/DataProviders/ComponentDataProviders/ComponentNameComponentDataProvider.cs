using System;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentNameComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            data.SetComponentName(type.Name.RemoveComponentSuffix());
        }
    }

    public static class ComponentNameComponentDataExtension {

        public const string COMPONENT_NAME = "Component.Name";

        public static string GetComponentName(this ComponentData data) {
            return (string)data[COMPONENT_NAME];
        }

        public static void SetComponentName(this ComponentData data, string name) {
            data[COMPONENT_NAME] = name;
        }
    }
}
