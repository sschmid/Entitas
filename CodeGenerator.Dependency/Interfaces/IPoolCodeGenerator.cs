namespace Entitas.CodeGeneration {
    public interface IPoolCodeGenerator : ICodeGenerator {
        CodeGenFile[] Generate(string[] poolNames);
    }
}

