public static class GameComponentsLookup
{
    public const int Animating = 0;
    public const int Asset = 1;
    public const int GameBoardElement = 2;
    public const int GameOver = 3;
    public const int Health = 4;
    public const int Highscore = 5;
    public const int Interactive = 6;
    public const int Movable = 7;
    public const int MyObject = 8;
    public const int Player = 9;
    public const int Position = 10;
    public const int User = 11;
    public const int Velocity = 12;
    public const int View = 13;

    public const int TotalComponents = 14;

    public static readonly string[] ComponentNames =
    {
        "Animating",
        "Asset",
        "GameBoardElement",
        "GameOver",
        "Health",
        "Highscore",
        "Interactive",
        "Movable",
        "MyObject",
        "Player",
        "Position",
        "User",
        "Velocity",
        "View"
    };

    public static readonly System.Type[] ComponentTypes =
    {
        typeof(AnimatingComponent),
        typeof(AssetComponent),
        typeof(GameBoardElementComponent),
        typeof(GameOverComponent),
        typeof(HealthComponent),
        typeof(HighscoreComponent),
        typeof(InteractiveComponent),
        typeof(MovableComponent),
        typeof(MyObjectComponent),
        typeof(PlayerComponent),
        typeof(PositionComponent),
        typeof(UserComponent),
        typeof(VelocityComponent),
        typeof(ViewComponent)
    };
}
