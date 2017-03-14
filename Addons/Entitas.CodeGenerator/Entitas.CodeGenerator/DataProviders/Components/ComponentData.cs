namespace Entitas.CodeGenerator {

    public class ComponentData : CodeGeneratorData {
    }


    public static class ComponentDataExtension {

        public static string ToComponentName(this string fullTypeName) {
            return fullTypeName.RemoveDots().RemoveComponentSuffix();
        }
    }
}
