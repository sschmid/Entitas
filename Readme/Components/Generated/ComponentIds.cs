using System.Collections.Generic;

public static class ComponentIds {
    public const int Animating = 0;
    public const int Health = 1;
    public const int Movable = 2;
    public const int Position = 3;
    public const int User = 4;

    public const int TotalComponents = 5;

    static readonly Dictionary<int, string> components = new Dictionary<int, string> {
        { 0, "Animating" },
        { 1, "Health" },
        { 2, "Movable" },
        { 3, "Position" },
        { 4, "User" }
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