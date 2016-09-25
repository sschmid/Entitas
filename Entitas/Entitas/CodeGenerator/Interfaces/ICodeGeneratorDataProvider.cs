namespace Entitas.CodeGenerator {

    public interface ICodeGeneratorDataProvider {

        ComponentInfo[] componentInfos { get; }

        // Expected behaviour:
        // - Make pool names distinct
        // - Sort pools by name
        // - Add CodeGenerator.DEFAULT_POOL_NAME if no custom pools are set
        string[] poolNames { get; }

        string[] blueprintNames { get; }
    }
}
