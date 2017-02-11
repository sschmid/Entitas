namespace Entitas.CodeGenerator {

    /// Implement this interface if you want to create a custom post processor.
    public interface ICodeGenFilePostProcessor : ICodeGeneratorInterface{

        void PostProcess(CodeGenFile[] files);
    }
}
