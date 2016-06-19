namespace Entitas.CodeGeneration {
    public interface IBlueprintsCodeGenerator : ICodeGenerator {
        CodeGenFile[] Generate(string[] blueprintNames);
    }
}

