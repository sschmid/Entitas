using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;
using Shouldly;

class describe_M0220 : nspec {

    void when_migrating() {

        var dir = TestExtensions.GetProjectRoot() + "/Tests/Tests/Tests/Entitas.Migration/Fixtures/M0220";

        IMigration m = null;

        before = () => {
            m = new M0220();
        };

        it["finds all reactive system"] = () => {
            var updatedFiles = m.Migrate(dir);
            updatedFiles.Length.ShouldBe(1);
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "RenderPositionSystem.cs")).ShouldBeTrue();
        };

        it["migrates to new api"] = () => {
            var updatedFiles = m.Migrate(dir);
            var reactiveSystemFile = updatedFiles.Single(file => file.fileName == Path.Combine(dir, "RenderPositionSystem.cs"));
            reactiveSystemFile.fileContent.ShouldBe(@"using System.Collections.Generic;
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
