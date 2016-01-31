public interface ICodeGeneratorDataProvider {
    string[] poolNames { get; }
    ComponentInfo[] componentInfos { get; }
}

