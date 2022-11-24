﻿using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Entitas.Migration.Tests
{
    public class M0220Tests
    {
        static string FixturePath => Path.Combine(TestExtensions.GetProjectRoot(), "tests", "Entitas.Migration.Tests", "fixtures", "exclude", "M0220");

        readonly M0220 _migration;

        public M0220Tests()
        {
            _migration = new M0220();
        }

        [Fact]
        public void FindsAllReactiveSystems()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            updatedFiles.Length.Should().Be(1);
            updatedFiles.Any(file => file.FileName == Path.Combine(FixturePath, "RenderPositionSystem.cs")).Should().BeTrue();
        }

        [Fact]
        public void MigratesToNewApi()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            var reactiveSystemFile = updatedFiles.Single(file => file.FileName == Path.Combine(FixturePath, "RenderPositionSystem.cs"));
            reactiveSystemFile.FileContent.Should().Be(@"using System.Collections.Generic;
using Entitas;

public class RenderPositionSystem : IReactiveSystem {
    public TriggerOnEvent trigger { get { return Matcher.AllOf(Matcher.Position, Matcher.View).OnEntityAdded(); } }


    public void Execute(List<Entity> entities) {
    }
}
");
        }
    }
}
