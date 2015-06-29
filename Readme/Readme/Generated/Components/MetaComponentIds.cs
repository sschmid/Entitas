using Entitas;

public static class MetaComponentIds {
    public const int Coins = 0;

    public const int TotalComponents = 1;

    static readonly string[] components = {
        "Coins"
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