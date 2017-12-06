using System;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ShouldGenerateComponentComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            var shouldGenerateComponent = !type.ImplementsInterface<IComponent>();
            data.ShouldGenerateComponent(shouldGenerateComponent);
            if (shouldGenerateComponent) {
                data.SetObjectType(type.ToCompilableString());
            }
        }
    }

    public static class ShouldGenerateComponentComponentDataExtension {

        public const string COMPONENT_GENERATE_COMPONENT = "Component.GenerateObjectComponent";
        public const string COMPONENT_OBJECT_TYPE = "Component.ObjectType";

        public static bool ShouldGenerateComponent(this ComponentData data) {
            return (bool)data[COMPONENT_GENERATE_COMPONENT];
        }

        public static void ShouldGenerateComponent(this ComponentData data, bool generate) {
            data[COMPONENT_GENERATE_COMPONENT] = generate;
        }

        public static string GetObjectType(this ComponentData data) {
            return (string)data[COMPONENT_OBJECT_TYPE];
        }

        public static void SetObjectType(this ComponentData data, string type) {
            data[COMPONENT_OBJECT_TYPE] = type;
        }
    }
}
