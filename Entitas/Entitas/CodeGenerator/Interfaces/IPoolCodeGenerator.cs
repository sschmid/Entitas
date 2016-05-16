namespace Entitas.CodeGenerator {
    public interface IPoolCodeGenerator : ICodeGenerator {
        CodeGenFile[] Generate(string[] poolNames, ComponentInfo[] infos);
    }
}

