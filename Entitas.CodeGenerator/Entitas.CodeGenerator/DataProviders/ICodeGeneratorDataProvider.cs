namespace Entitas.CodeGenerator {

    /// Implement this interface if you want to provide custom data
    /// for your custom code generators.
    public interface ICodeGeneratorDataProvider {

        CodeGeneratorData[] GetData();
    }
}
