public static class GameComponentsLookup {

    public const int Age = 0;
    public const int Name = 1;

    public const int TotalComponents = 2;

    public static readonly string[] componentNames = {
        "Age",
        "Name"
    };

    public static readonly System.Type[] componentTypes = {
        typeof(AgeComponent),
        typeof(NameComponent)
    };
}
