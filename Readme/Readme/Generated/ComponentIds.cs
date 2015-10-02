public static class ComponentIds {
    public const int Animating = 0;
    public const int GameBoardElement = 1;
    public const int Health = 2;
    public const int Interactive = 3;
    public const int Movable = 4;
    public const int Move = 5;
    public const int Position = 6;
    public const int Resource = 7;
    public const int User = 8;

    public const int TotalComponents = 9;

    static readonly string[] components = {
        "Animating",
        "GameBoardElement",
        "Health",
        "Interactive",
        "Movable",
        "Move",
        "Position",
        "Resource",
        "User"
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}