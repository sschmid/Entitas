using System.Collections.Generic;

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

    static readonly Dictionary<int, string> components = new Dictionary<int, string> {
        { 0, "Animating" },
        { 1, "GameBoardElement" },
        { 2, "Health" },
        { 3, "Interactive" },
        { 4, "Movable" },
        { 5, "Move" },
        { 6, "Position" },
        { 7, "Resource" },
        { 8, "User" }
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

namespace Entitas {
    public partial class Matcher : AllOfMatcher {
        public Matcher(int index) : base(new [] { index }) {
        }

        public override string ToString() {
            return ComponentIds.IdToString(indices[0]);
        }
    }
}