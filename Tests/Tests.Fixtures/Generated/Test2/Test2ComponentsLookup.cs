public static class Test2ComponentsLookup {

    public const int ClassToGenerate = 0;
    public const int MyNamespace = 1;
    public const int NameAge = 2;
    public const int Test2Context = 3;

    public const int TotalComponents = 4;

    public static readonly string[] componentNames = {
        "ClassToGenerate",
        "MyNamespace",
        "NameAge",
        "Test2Context"
    };

    public static readonly System.Type[] componentTypes = {
        typeof(ClassToGenerateComponent),
        typeof(My.Namespace.MyNamespaceComponent),
        typeof(NameAgeComponent),
        typeof(Test2ContextComponent)
    };
}
