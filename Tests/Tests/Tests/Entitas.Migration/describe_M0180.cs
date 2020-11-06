using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;
using Shouldly;

class describe_M0180 : nspec {

    void when_migration() {

        var dir = TestExtensions.GetProjectRoot() + "/Tests/Tests/Tests/Entitas.Migration/Fixtures/M0180";

        IMigration m = null;

        before = () => {
            m = new M0180();
        };

        it["finds all reactive system"] = () => {
            var updatedFiles = m.Migrate(dir);
            updatedFiles.Length.ShouldBe(3);
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "RenderPositionSystem.cs")).ShouldBeTrue();
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "RenderRotationSystem.cs")).ShouldBeTrue();
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, Path.Combine("SubFolder", "RenderSelectedSystem.cs"))).ShouldBeTrue();
        };

        it["migrates to new api"] = () => {
            var updatedFiles = m.Migrate(dir);

            var reactiveSystemFile = updatedFiles.Where(file => file.fileName == Path.Combine(dir, "RenderRotationSystem.cs")).First();
            reactiveSystemFile.fileContent.ShouldBe(@"using Entitas;

public class RenderRotationSystem : IReactiveSystem
{
    public IMatcher trigger { get { return Matcher.AllOf(CoreMatcher.Rotation, CoreMatcher.View);
    } }

    public GroupEventType eventType { get { return GroupEventType.OnEntityAdded;
    } }

    public void Execute(Entity[] entities) {
        // Do sth
    }
}
");
        };
    }
}
