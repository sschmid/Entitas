namespace Entitas.CodeGeneration {

    /// Implement this interface if you want to create a custom code generator.
    public interface ICodeGenerator : ICodeGeneratorInterface {

        CodeGenFile[] Generate(CodeGeneratorData[] data);
    }
}
