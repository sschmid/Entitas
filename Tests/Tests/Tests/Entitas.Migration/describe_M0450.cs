using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;

class describe_M0450 : nspec {

    void when_migrating() {

        var dir = TestExtensions.GetProjectRoot() + "/Tests/Tests/Tests/Entitas.Migration/Fixtures/M0450";

        IMigration m = null;

        before = () => {
            m = new M0450();
        };

        it["finds all files"] = () => {
            var updatedFiles = m.Migrate(dir);
            updatedFiles.Length.should_be(1);
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "Entitas.properties")).should_be_true();
        };

        it["updates Entitas.properties"] = () => {
            var updatedFiles = m.Migrate(dir);
            var file = updatedFiles[0];

            file.fileContent.Contains("Entitas.CodeGeneration.CodeGenerator.SearchPaths").should_be_false();
            file.fileContent.Contains("CodeGenerator.SearchPaths").should_be_true();

            file.fileContent.Contains("Entitas.CodeGeneration.CodeGenerator.Plugins").should_be_false();
            file.fileContent.Contains("CodeGenerator.Plugins").should_be_true();

            file.fileContent.Contains("Entitas.CodeGeneration.CodeGenerator.DataProviders").should_be_false();
            file.fileContent.Contains("CodeGenerator.DataProviders").should_be_true();

            file.fileContent.Contains("Entitas.CodeGeneration.CodeGenerator.CodeGenerators").should_be_false();
            file.fileContent.Contains("CodeGenerator.CodeGenerators").should_be_true();

            file.fileContent.Contains("Entitas.CodeGeneration.CodeGenerator.PostProcessors").should_be_false();
            file.fileContent.Contains("CodeGenerator.PostProcessors").should_be_true();
        };
    }
}
