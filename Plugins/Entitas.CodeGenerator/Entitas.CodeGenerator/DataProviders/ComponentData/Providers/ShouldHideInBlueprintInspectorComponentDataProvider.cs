using System;
using System.Linq;
using Entitas.CodeGenerator.Api;

namespace Entitas.CodeGenerator {

    public class ShouldHideInBlueprintInspectorComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            var hide = Attribute
                .GetCustomAttributes(type)
                .OfType<HideInBlueprintInspectorAttribute>()
                .Any();

            data.ShouldHideInBlueprintInspector(hide);
        }
    }

    public static class ShouldHideInBlueprintInspectorComponentDataProviderExtension {

        public const string COMPONENT_HIDE_IN_BLUEPRINT_INSPECTOR = "component_HideInBlueprintInspector";

        public static bool ShouldHideInBlueprintInspector(this ComponentData data) {
            return (bool)data[COMPONENT_HIDE_IN_BLUEPRINT_INSPECTOR];
        }

        public static void ShouldHideInBlueprintInspector(this ComponentData data, bool hide) {
            data[COMPONENT_HIDE_IN_BLUEPRINT_INSPECTOR] = hide;
        }
    }
}
