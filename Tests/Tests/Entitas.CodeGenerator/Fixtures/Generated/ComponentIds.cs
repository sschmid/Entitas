public static class ComponentIds {
    public const int Animating = 0;
    public const int ComponentA = 1;
    public const int ComponentB = 2;
    public const int ComponentC = 3;
    public const int ComponentD = 4;
    public const int ComponentE = 5;
    public const int ComponentF = 6;
    public const int CustomPrefix = 7;
    public const int DontGenerate = 8;
    public const int Movable = 9;
    public const int Namespace = 10;
    public const int Person = 11;
    public const int Some = 12;
    public const int User = 13;
    public const int View = 14;

    public const int TotalComponents = 15;

    public static readonly string[] componentNames = {
        "Animating",
        "ComponentA",
        "ComponentB",
        "ComponentC",
        "ComponentD",
        "ComponentE",
        "ComponentF",
        "CustomPrefix",
        "DontGenerate",
        "Movable",
        "Namespace",
        "Person",
        "Some",
        "User",
        "View"
    };

    public static readonly System.Type[] componentTypes = {
        typeof(AnimatingComponent),
        typeof(ComponentA),
        typeof(ComponentB),
        typeof(ComponentC),
        typeof(ComponentD),
        typeof(ComponentE),
        typeof(ComponentF),
        typeof(CustomPrefixComponent),
        typeof(DontGenerateComponent),
        typeof(MovableComponent),
        typeof(My.Namespace.NamespaceComponent),
        typeof(PersonComponent),
        typeof(SomeComponent),
        typeof(UserComponent),
        typeof(View)
    };
}