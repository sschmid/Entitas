using Entitas;

using System.Collections.Generic;

public static class MetaComponentIds {
    public const int Coins = 0;

    public const int TotalComponents = 1;

    static readonly Dictionary<int, string> components = new Dictionary<int, string> {
        { 0, "Coins" }
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

public partial class MetaMatcher : AllOfMatcher {
    public MetaMatcher(int index) : base(new [] { index }) {
    }

    public override string ToString() {
        return MetaComponentIds.IdToString(indices[0]);
    }
}