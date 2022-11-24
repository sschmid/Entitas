﻿using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Entitas.Migration.Tests
{
    public class M0180Tests
    {
        static string FixturePath => Path.Combine(TestExtensions.GetProjectRoot(), "tests", "Entitas.Migration.Tests", "fixtures", "exclude", "M0180");

        readonly M0180 _migration;

        public M0180Tests()
        {
            _migration = new M0180();
        }

        [Fact]
        public void FindsAllReactiveSystems()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            updatedFiles.Length.Should().Be(3);
            updatedFiles.Any(file => file.FileName == Path.Combine(FixturePath, "RenderPositionSystem.cs")).Should().BeTrue();
            updatedFiles.Any(file => file.FileName == Path.Combine(FixturePath, "RenderRotationSystem.cs")).Should().BeTrue();
            updatedFiles.Any(file => file.FileName == Path.Combine(FixturePath, Path.Combine("SubFolder", "RenderSelectedSystem.cs"))).Should().BeTrue();
        }

        [Fact]
        public void MigratesToNewApi()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            var reactiveSystemFile = updatedFiles.First(file => file.FileName == Path.Combine(FixturePath, "RenderRotationSystem.cs"));
            reactiveSystemFile.FileContent.Should().Be(@"using Entitas;

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
        }
    }
}
