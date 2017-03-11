public static class GameStateComponentsLookup {

    public const int Score = 0;

    public const int TotalComponents = 1;

    public static readonly string[] componentNames = {
        "Score"
    };

    public static readonly System.Type[] componentTypes = {
        typeof(ScoreComponent)
    };
}
