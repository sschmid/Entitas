namespace Entitas.CodeGeneration {

    /// Implement this interface if you want to create a custom post processor.
    public interface ICodeGenFilePostProcessor : ICodeGeneratorInterface {

        CodeGenFile[] PostProcess(CodeGenFile[] files);
    }
}
