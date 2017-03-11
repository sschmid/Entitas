public static class GameComponentsLookup {

    public const int Asset = 0;
    public const int GameBoardElement = 1;
    public const int Interactive = 2;
    public const int Movable = 3;
    public const int Position = 4;
    public const int Velocity = 5;
    public const int View = 6;

    public const int TotalComponents = 7;

    public static readonly string[] componentNames = {
        "Asset",
        "GameBoardElement",
        "Interactive",
        "Movable",
        "Position",
        "Velocity",
        "View"
    };

    public static readonly System.Type[] componentTypes = {
        typeof(AssetComponent),
        typeof(GameBoardElementComponent),
        typeof(InteractiveComponent),
        typeof(MovableComponent),
        typeof(PositionComponent),
        typeof(VelocityComponent),
        typeof(ViewComponent)
    };
}
