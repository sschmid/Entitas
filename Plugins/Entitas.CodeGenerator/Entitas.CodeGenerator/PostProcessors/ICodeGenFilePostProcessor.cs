namespace Entitas.CodeGenerator {

    /// Implement this interface if you want to create a custom post processor.
    public interface ICodeGenFilePostProcessor {

        bool IsEnabledByDefault { get; }

        void PostProcess(CodeGenFile[] files);
    }
}
