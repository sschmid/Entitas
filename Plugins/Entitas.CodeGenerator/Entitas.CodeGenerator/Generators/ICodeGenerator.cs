namespace Entitas.CodeGenerator {

    /// Implement this interface if you want to create a custom code generator.
    public interface ICodeGenerator {

        bool IsEnabledByDefault { get; }

        CodeGenFile[] Generate(CodeGeneratorData[] data);
    }
}
