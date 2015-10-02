using Entitas;

public static class TestComponentIds {
    public const int Test = 0;

    public const int TotalComponents = 1;

    static readonly string[] _components = {
        "Test"
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}