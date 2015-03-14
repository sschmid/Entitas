using System.Collections.Generic;

public static class ComponentIds {
    public const int Name = 0;
    public const int Test = 1;

    public const int TotalComponents = 2;

    static readonly Dictionary<int, string> components = new Dictionary<int, string> {
        { 0, "Name" },
        { 1, "Test" }
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

namespace Entitas {
    public partial class Pool {
        public Pool() : this(ComponentIds.TotalComponents) {
        }
    }
}

namespace Entitas {
    public partial class Matcher : AllOfMatcher {
        public Matcher(int index) : base(new [] { index }) {
        }

        public override string ToString() {
            return string.Format("Matcher(" + ComponentIds.IdToString(indices[0]) + ")");
        }
    }
}