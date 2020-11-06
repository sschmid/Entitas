using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;
using Shouldly;

class describe_MigrationUtils : nspec {

    void when_migrating() {

        var dir = TestExtensions.GetProjectRoot() + "/Tests/Tests/Tests/Entitas.Migration/Fixtures/M0180";

        it["gets only *.cs source files"] = () => {
            var files = MigrationUtils.GetFiles(dir);
            files.Length.ShouldBe(6);
            files.Any(file => file.fileName == Path.Combine(dir, "SourceFile.cs")).ShouldBeTrue();
            files.Any(file => file.fileName == Path.Combine(dir, Path.Combine("SubFolder", "SourceFile2.cs"))).ShouldBeTrue();
            files.Any(file => file.fileName == Path.Combine(dir, "RenderPositionSystem.cs")).ShouldBeTrue();
            files.Any(file => file.fileName == Path.Combine(dir, "RenderRotationSystem.cs")).ShouldBeTrue();
            files.Any(file => file.fileName == Path.Combine(dir, Path.Combine("SubFolder", "RenderSelectedSystem.cs"))).ShouldBeTrue();
            files.Any(file => file.fileName == Path.Combine(dir, Path.Combine("SubFolder", "MoveSystem.cs"))).ShouldBeTrue();
        };
    }
}
