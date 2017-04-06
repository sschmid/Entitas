namespace Entitas.CodeGeneration {

    /// Implement this interface if you want to provide custom data
    /// for your custom code generators.
    public interface ICodeGeneratorDataProvider : ICodeGeneratorInterface {

        CodeGeneratorData[] GetData();
    }
}
