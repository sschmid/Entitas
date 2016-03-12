using System;
using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;

class describe_M0300 : nspec {
    void when_migrating() {

        var dir = Environment.CurrentDirectory + "/Tests/Tests/Entitas.Migration/Fixtures/M0300";

        IMigration m = null;
        before = () => {
            m = new M0300();
        };

        it["finds all files"] = () => {
            var updatedFiles = m.Migrate(dir);
            updatedFiles.Length.should_be(1);
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "Entitas.properties")).should_be_true();
        };

        it["updates Entitas.properties"] = () => {
            var updatedFiles = m.Migrate(dir);
            updatedFiles[0].fileContent.Contains("ComponentsGenerator").should_be_false();
            updatedFiles[0].fileContent.Contains("ComponentExtensionsGenerator").should_be_true();
        };
    }
}

