using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;
using Shouldly;

class describe_M0300 : nspec {

    void when_migrating() {

        var dir = TestExtensions.GetProjectRoot() + "/Tests/Tests/Tests/Entitas.Migration/Fixtures/M0300";

        IMigration m = null;

        before = () => {
            m = new M0300();
        };

        it["finds all files"] = () => {
            var updatedFiles = m.Migrate(dir);
            updatedFiles.Length.ShouldBe(1);
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "Entitas.properties")).ShouldBeTrue();
        };

        it["updates Entitas.properties"] = () => {
            var updatedFiles = m.Migrate(dir);
            var file = updatedFiles[0];

            file.fileContent.Contains("ComponentsGenerator").ShouldBeFalse();
            file.fileContent.Contains("ComponentExtensionsGenerator").ShouldBeTrue();

            file.fileContent.Contains("PoolAttributeGenerator").ShouldBeFalse();
            file.fileContent.Contains("PoolAttributesGenerator").ShouldBeTrue();
        };
    }
}
