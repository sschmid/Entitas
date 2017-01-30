namespace Entitas.CodeGenerator {

    /// Implement this interface if you want to create a custom code generator.
    public interface ICodeGenerator {

        CodeGenFile[] Generate(CodeGeneratorData[] data);
    }
}
