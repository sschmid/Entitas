using System.Collections.Generic;

public static class ComponentIds {
    public const int Position = 0;
    public const int Health = 1;
    public const int Movable = 2;
    public const int User = 3;
    public const int Animating = 4;

    public const int TotalComponents = 5;

    static readonly Dictionary<int, string> components = new Dictionary<int, string> {
        { 0, "Position" },
        { 1, "Health" },
        { 2, "Movable" },
        { 3, "User" },
        { 4, "Animating" }
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
            return string.Format("Matcher_" + ComponentIds.IdToString(indices[0]));
        }
    }
}