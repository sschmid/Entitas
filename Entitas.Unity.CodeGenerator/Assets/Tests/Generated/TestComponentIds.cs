using Entitas;

using System.Collections.Generic;

public static class TestComponentIds {
    public const int Test = 0;

    public const int TotalComponents = 1;

    static readonly Dictionary<int, string> components = new Dictionary<int, string> {
        { 0, "Test" }
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

public partial class TestMatcher : AllOfMatcher {
    public TestMatcher(int index) : base(new [] { index }) {
    }

    public override string ToString() {
        return TestComponentIds.IdToString(indices[0]);
    }
}