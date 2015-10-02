public static class ComponentIds {

    public const int TotalComponents = 0;

    static readonly string[] _components = {
    };

    public static string IdToString(int componentId) {
        return _components[componentId];
    }
}