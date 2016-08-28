using System;
using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;

class describe_M0220 : nspec {

    void when_migrating() {

        var dir = Environment.CurrentDirectory + "/Tests/Tests/Entitas.Migration/Fixtures/M0220";

        IMigration m = null;
        before = () => {
            m = new M0220();
        };

        it["finds all reactive system"] = () => {
            var updatedFiles = m.Migrate(dir);
            updatedFiles.Length.should_be(1);
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "RenderPositionSystem.cs")).should_be_true();
        };

        it["migrates to new api"] = () => {
            var updatedFiles = m.Migrate(dir);
            var reactiveSystemFile = updatedFiles.Single(file => file.fileName == Path.Combine(dir, "RenderPositionSystem.cs"));
            reactiveSystemFile.fileContent.should_be(@"using System.Collections.Generic;
using Entitas;

public class RenderPositionSystem : IReactiveSystem {
    public TriggerOnEvent trigger { get { return Matcher.AllOf(Matcher.Position, Matcher.View).OnEntityAdded(); } }


    public void Execute(List<Entity> entities) {
    }
}
");
        };

    }
}

