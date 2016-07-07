namespace Entitas.CodeGenerator {
    public interface ICodeGeneratorDataProvider {
        string[] poolNames { get; }
        ComponentInfo[] componentInfos { get; }
        string[] blueprintNames { get; }
    }
}

