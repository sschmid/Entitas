using Entitas;

public static class UIComponentIds {
    public const int UIPosition = 0;

    public const int TotalComponents = 1;

    static readonly string[] components = {
        "UIPosition"
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}