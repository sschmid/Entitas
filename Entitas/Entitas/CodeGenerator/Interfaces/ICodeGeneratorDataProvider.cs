namespace Entitas.CodeGenerator {

    public interface ICodeGeneratorDataProvider {

        ComponentInfo[] componentInfos { get; }

        // Expected behaviour:
        // - Make context names distinct
        // - Sort contexts by name
        // - Add CodeGenerator.DEFAULT_CONTEXT_NAME if no custom contexts are set
        string[] contextNames { get; }

        string[] blueprintNames { get; }
    }
}
