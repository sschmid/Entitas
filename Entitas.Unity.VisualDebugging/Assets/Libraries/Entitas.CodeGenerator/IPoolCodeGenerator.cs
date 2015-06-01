namespace Entitas.CodeGenerator {
    public interface IPoolCodeGenerator {
        CodeGenFile[] Generate(string[] poolNames);
    }
}

