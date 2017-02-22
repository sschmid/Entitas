namespace Entitas.CodeGenerator {

    /// Implement this interface if you want to create a custom post processor.
    public interface ICodeGenFilePostProcessor : ICodeGeneratorInterface{

        int priority { get; }
        CodeGenFile[] PostProcess(CodeGenFile[] files);
    }
}
