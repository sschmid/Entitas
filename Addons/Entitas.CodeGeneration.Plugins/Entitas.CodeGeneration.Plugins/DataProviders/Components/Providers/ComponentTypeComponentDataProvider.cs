using System;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentTypeComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            data.SetTypeName(type.ToCompilableString());
        }
    }

    public static class ComponentTypeComponentDataExtension {

        public const string COMPONENT_TYPE = "Component.TypeName";

        public static string GetTypeName(this ComponentData data) {
            return (string)data[COMPONENT_TYPE];
        }

        public static void SetTypeName(this ComponentData data, string fullTypeName) {
            data[COMPONENT_TYPE] = fullTypeName;
        }
    }
}
