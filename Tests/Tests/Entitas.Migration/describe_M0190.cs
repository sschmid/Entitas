using System;
using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;

class describe_M0190 : nspec {

    void when_migrating() {

        var dir = Environment.CurrentDirectory + "/Tests/Tests/Entitas.Migration/Fixtures/M0190";

        IMigration m = null;
        before = () => {
            m = new M0190();
        };

        it["finds all reactive system"] = () => {
            var updatedFiles = m.Migrate(dir);
            updatedFiles.Length.should_be(3);
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "RenderPositionSystem.cs")).should_be_true();
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "RenderRotationSystem.cs")).should_be_true();
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, Path.Combine("SubFolder", "RenderSelectedSystem.cs"))).should_be_true();
        };

        it["migrates to new api"] = () => {
            var updatedFiles = m.Migrate(dir);
            var reactiveSystemFile = updatedFiles.Where(file => file.fileName == Path.Combine(dir, "RenderRotationSystem.cs")).First();
            reactiveSystemFile.fileContent.should_be(@"using Entitas;

public class RenderRotationSystem : IReactiveSystem {
    public IMatcher trigger { get { return Matcher.AllOf(CoreMatcher.Rotation, CoreMatcher.View); } }

    public GroupEventType eventType { get { return GroupEventType.OnEntityAdded; } }

    public void Execute(System.Collections.Generic.List<Entity> entities) {
        // Do sth
    }
}
");
        };

    }
}

