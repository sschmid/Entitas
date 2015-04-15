using Entitas;

using System.Collections.Generic;

public static class UIComponentIds {
    public const int UIPosition = 0;

    public const int TotalComponents = 1;

    static readonly Dictionary<int, string> components = new Dictionary<int, string> {
        { 0, "UIPosition" }
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

public partial class UIMatcher : AllOfMatcher {
    public UIMatcher(int index) : base(new [] { index }) {
    }

    public override string ToString() {
        return string.Format("UI_" + UIComponentIds.IdToString(indices[0]));
    }
}