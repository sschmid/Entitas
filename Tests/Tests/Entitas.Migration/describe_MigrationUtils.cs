using System;
using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;

class describe_MigrationUtils : nspec {
    void when_migrating() {

        var dir = Environment.CurrentDirectory + "/Tests/Tests/Entitas.Migration/Fixtures/M0180";

        it["gets only *.cs source files"] = () => {
            var files = MigrationUtils.GetFiles(dir);
            files.Length.should_be(6);
            files.Any(file => file.fileName == Path.Combine(dir, "SourceFile.cs")).should_be_true();
            files.Any(file => file.fileName == Path.Combine(dir, Path.Combine("SubFolder", "SourceFile2.cs"))).should_be_true();
            files.Any(file => file.fileName == Path.Combine(dir, "RenderPositionSystem.cs")).should_be_true();
            files.Any(file => file.fileName == Path.Combine(dir, "RenderRotationSystem.cs")).should_be_true();
            files.Any(file => file.fileName == Path.Combine(dir, Path.Combine("SubFolder", "RenderSelectedSystem.cs"))).should_be_true();
            files.Any(file => file.fileName == Path.Combine(dir, Path.Combine("SubFolder", "MoveSystem.cs"))).should_be_true();
        };
    }
}

