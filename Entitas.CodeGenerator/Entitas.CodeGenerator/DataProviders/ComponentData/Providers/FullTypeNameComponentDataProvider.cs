using System;
using Entitas.Utils;

namespace Entitas.CodeGenerator {

    public class FullTypeNameComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            data.SetFullTypeName(type.ToCompilableString());
        }
    }

    public static class FullTypeNameComponentDataProviderExtension {

        public const string COMPONENT_FULL_TYPE_NAME = "component_fullTypeName";

        public static string GetFullTypeName(this ComponentData data) {
            return (string)data[COMPONENT_FULL_TYPE_NAME];
        }

        public static void SetFullTypeName(this ComponentData data, string fullTypeName) {
            data[COMPONENT_FULL_TYPE_NAME] = fullTypeName;
        }

        public static string GetShortTypeName(this ComponentData data) {
            var fullTypeName = GetFullTypeName(data);
            var nameSplit = fullTypeName.Split('.');
            return nameSplit[nameSplit.Length - 1];
        }
    }
}
