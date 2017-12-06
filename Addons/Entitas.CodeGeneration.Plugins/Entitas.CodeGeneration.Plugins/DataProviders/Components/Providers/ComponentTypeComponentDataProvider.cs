using System;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentTypeComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            data.SetlTypeName(type.ToCompilableString());
        }
    }

    public static class ComponentTypeComponentDataExtension {

        public const string COMPONENT_TYPE = "Component.ComponentType";

        public static string GetTypeName(this ComponentData data) {
            return (string)data[COMPONENT_TYPE];
        }

        public static void SetlTypeName(this ComponentData data, string fullTypeName) {
            data[COMPONENT_TYPE] = fullTypeName;
        }
    }
}
