using System.Collections.Generic;
using System.IO;
using Entitas;
using NSpec;

public static class TestExtensions {

    public static void Fail(this nspec spec) {
        "but did".should_be("should not happen");
    }

    public static Entity CreateEntity(this nspec spec) {
        return new Entity(CID.TotalComponents, new Stack<IComponent>[CID.TotalComponents]);
    }

    public static string GetProjectRoot() {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        if(current.Parent.Parent.Name == "Tests") {
            // This happens if you run the TestRunner from your IDE
            return current.Parent.Parent.Parent.FullName;
        }

        // This happens if you use the provided runTests.sh
        return current.FullName;
    }
}
