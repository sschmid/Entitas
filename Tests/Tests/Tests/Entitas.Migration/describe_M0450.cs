using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;
using Shouldly;

class describe_M0450 : nspec {

    void when_migrating() {

        var dir = TestExtensions.GetProjectRoot() + "/Tests/Tests/Tests/Entitas.Migration/Fixtures/M0450";

        IMigration m = null;

        before = () => {
            m = new M0450();
        };

        it["finds all files"] = () => {
            var updatedFiles = m.Migrate(dir);
            updatedFiles.Length.ShouldBe(1);
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "Entitas.properties")).ShouldBeTrue();
        };

        it["updates Entitas.properties"] = () => {
            var updatedFiles = m.Migrate(dir);
            var file = updatedFiles[0];

            file.fileContent.Contains("Entitas.CodeGeneration.CodeGenerator.SearchPaths").ShouldBeFalse();
            file.fileContent.Contains("CodeGenerator.SearchPaths").ShouldBeTrue();

            file.fileContent.Contains("Entitas.CodeGeneration.CodeGenerator.Plugins").ShouldBeFalse();
            file.fileContent.Contains("CodeGenerator.Plugins").ShouldBeTrue();

            file.fileContent.Contains("Entitas.CodeGeneration.CodeGenerator.DataProviders").ShouldBeFalse();
            file.fileContent.Contains("CodeGenerator.DataProviders").ShouldBeTrue();

            file.fileContent.Contains("Entitas.CodeGeneration.CodeGenerator.CodeGenerators").ShouldBeFalse();
            file.fileContent.Contains("CodeGenerator.CodeGenerators").ShouldBeTrue();

            file.fileContent.Contains("Entitas.CodeGeneration.CodeGenerator.PostProcessors").ShouldBeFalse();
            file.fileContent.Contains("CodeGenerator.PostProcessors").ShouldBeTrue();

            file.fileContent.Contains("Entitas.CodeGeneration.CodeGenerator.CLI.Ignore.UnusedKeys").ShouldBeFalse();
            file.fileContent.Contains("CodeGenerator.CLI.Ignore.UnusedKeys").ShouldBeTrue();
        };
    }
}
