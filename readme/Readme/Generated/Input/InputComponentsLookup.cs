public static class InputComponentsLookup {

    public const int Animating = 0;
    public const int Input = 1;
    public const int User = 2;

    public const int TotalComponents = 3;

    public static readonly string[] componentNames = {
        "Animating",
        "Input",
        "User"
    };

    public static readonly System.Type[] componentTypes = {
        typeof(AnimatingComponent),
        typeof(InputComponent),
        typeof(UserComponent)
    };
}
