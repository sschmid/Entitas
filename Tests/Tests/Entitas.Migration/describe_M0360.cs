using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;

class describe_M0360 : nspec {

    void when_migrating() {

        var dir = TestExtensions.GetProjectRoot() + "/Tests/Tests/Entitas.Migration/Fixtures/M0360";

        IMigration m = null;

        before = () => {
            m = new M0360();
        };

        it["finds all files"] = () => {
            var updatedFiles = m.Migrate(dir);
            updatedFiles.Length.should_be(1);
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "Entitas.properties")).should_be_true();
        };

        it["updates Entitas.properties"] = () => {
            var updatedFiles = m.Migrate(dir);
            var file = updatedFiles[0];

            file.fileContent.Contains("Entitas.CodeGenerator.Pools").should_be_false();
            file.fileContent.Contains("Entitas.CodeGenerator.Contexts").should_be_true();
        };
    }
}
