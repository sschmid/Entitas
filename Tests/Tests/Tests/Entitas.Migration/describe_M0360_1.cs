using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;
using Shouldly;

class describe_M0360_1 : nspec {

    void when_migrating() {

        var dir = TestExtensions.GetProjectRoot() + "/Tests/Tests/Tests/Entitas.Migration/Fixtures/M0360";

        IMigration m = null;

        before = () => {
            m = new M0360_1();
        };

        it["finds all files"] = () => {
            var updatedFiles = m.Migrate(dir);
            updatedFiles.Length.ShouldBe(1);
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "Entitas.properties")).ShouldBeTrue();
        };

        it["updates Entitas.properties"] = () => {
            var updatedFiles = m.Migrate(dir);
            var file = updatedFiles[0];

            file.fileContent.Contains("Entitas.CodeGenerator.Pools").ShouldBeFalse();
            file.fileContent.Contains("Entitas.CodeGenerator.Contexts").ShouldBeTrue();
        };
    }
}
