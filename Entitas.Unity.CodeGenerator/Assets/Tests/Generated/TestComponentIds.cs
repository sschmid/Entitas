using Entitas;

public static class TestComponentIds {
    public const int Test = 0;

    public const int TotalComponents = 1;

    public static readonly string[] componentNames = {
        "Test"
    };

    public static readonly System.Type[] componentTypes = {
        typeof(TestComponent)
    };
}