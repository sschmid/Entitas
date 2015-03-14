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

public partial class MetaPool : Pool {
    public MetaPool() : base(MetaComponentIds.TotalComponents) {
    }

    public MetaPool(int startCreationIndex) : base(MetaComponentIds.TotalComponents, startCreationIndex) {
    }
}

public partial class MetaMatcher : AllOfMatcher {
    public MetaMatcher(int index) : base(new [] { index }) {
    }

    public override string ToString() {
        return string.Format("Meta(" + MetaComponentIds.IdToString(indices[0]) + ")");
    }
}