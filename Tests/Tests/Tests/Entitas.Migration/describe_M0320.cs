using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;
using Shouldly;

class describe_M0320 : nspec {

    void when_migrating() {

        var dir = TestExtensions.GetProjectRoot() + "/Tests/Tests/Tests/Entitas.Migration/Fixtures/M0320";

        IMigration m = null;

        before = () => {
            m = new M0320();
        };

        it["finds all files"] = () => {
            var updatedFiles = m.Migrate(dir);
            updatedFiles.Length.ShouldBe(2);
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "Entitas.properties")).ShouldBeTrue();
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "Systems.cs")).ShouldBeTrue();
        };

        it["updates Entitas.properties"] = () => {
            var updatedFiles = m.Migrate(dir);
            var file = updatedFiles[0];

            file.fileContent.Contains("Entitas.Unity.CodeGenerator.GeneratedFolderPath").ShouldBeFalse();
            file.fileContent.Contains("Entitas.CodeGenerator.GeneratedFolderPath").ShouldBeTrue();

            file.fileContent.Contains("Entitas.Unity.CodeGenerator.Pools").ShouldBeFalse();
            file.fileContent.Contains("Entitas.CodeGenerator.Pools").ShouldBeTrue();

            file.fileContent.Contains("Entitas.Unity.CodeGenerator.EnabledCodeGenerators").ShouldBeFalse();
            file.fileContent.Contains("Entitas.CodeGenerator.EnabledCodeGenerators").ShouldBeTrue();

            // Ignores Entitas.Unity.VisualDebugging
            file.fileContent.Contains("Entitas.Unity.VisualDebugging.DefaultInstanceCreatorFolderPath").ShouldBeTrue();
            file.fileContent.Contains("Entitas.Unity.VisualDebugging.TypeDrawerFolderPath").ShouldBeTrue();
        };

        it["updates pool.CreateSystem(instance)"] = () => {
            var updatedFiles = m.Migrate(dir);
            var file = updatedFiles[1];
            file.fileContent.ShouldBe("pool.CreateSystem(new MySystem1());\npool.CreateSystem(new MySystem2());\n");
        };
    }
}
