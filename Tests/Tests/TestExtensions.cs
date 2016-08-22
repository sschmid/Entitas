using System.Collections.Generic;
using Entitas;
using NSpec;

public static class TestExtensions {

    public static void Fail(this nspec spec) {
        "but did".should_be("should not execute");
    }

    public static Entity CreateEntity(this nspec spec) {
        return new Entity(CID.NumComponents, new Stack<IComponent>[CID.NumComponents]);
    }
}

