namespace Entitas.CodeGenerator {

    public interface ICodeGenFilePostProcessor {

        void PostProcess(CodeGenFile[] files);
    }
}
