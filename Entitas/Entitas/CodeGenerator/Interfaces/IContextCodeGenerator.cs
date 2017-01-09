namespace Entitas.CodeGenerator {

    public interface IContextCodeGenerator : ICodeGenerator {

        CodeGenFile[] Generate(string[] contextNames);
    }
}
