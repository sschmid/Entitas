using Entitas;
using NSpec;
using System.Collections.Generic;

public static class EntityExtensions {
    public static bool IsEnabled(this Entity entity) {
        return entity.isEnabled;
    }
}

public static class TestExtensions {
    public static void Fail(this nspec spec) {
        "but did".should_be("should not execute");
    }

    public static Entity CreateEntity(this nspec spec) {
        return new Entity(CID.NumComponents, new Stack<IComponent>[CID.NumComponents]);
    }
}

