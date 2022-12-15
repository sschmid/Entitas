public static class GameStateComponentsLookup
{
    public const int Score = 0;

    public const int TotalComponents = 1;

    public static readonly string[] ComponentNames =
    {
        "Score"
    };

    public static readonly System.Type[] ComponentTypes =
    {
        typeof(ScoreComponent)
    };
}
