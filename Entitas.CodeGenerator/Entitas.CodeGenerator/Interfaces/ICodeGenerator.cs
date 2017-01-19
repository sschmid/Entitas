namespace Entitas.CodeGenerator {

    public interface ICodeGenerator {

        CodeGenFile[] Generate(CodeGeneratorData[] data);
    }
}
