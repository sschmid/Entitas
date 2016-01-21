public static class PoolCComponentIds {
    public const int C = 0;
    public const int E = 1;
    public const int D = 2;
    public const int F = 3;

    public const int TotalComponents = 4;

    public static readonly string[] componentNames = {
        "C",
        "E",
        "D",
        "F"
    };

    public static readonly System.Type[] componentTypes = {
        typeof(CComponent),
        typeof(EComponent),
        typeof(DComponent),
        typeof(FComponent)
    };
}