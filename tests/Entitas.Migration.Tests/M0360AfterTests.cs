using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Entitas.Migration.Tests
{
    public class M0360AfterTests
    {
        static string FixturesPath => Path.Combine(TestExtensions.GetProjectRoot(), "tests", "Entitas.Migration.Tests", "fixtures", "exclude", "M0360");

        readonly ITestOutputHelper _output;
        readonly M0360_2 _migration;

        public M0360AfterTests(ITestOutputHelper output)
        {
            _output = output;
            _migration = new M0360_2();
        }

        [Fact]
        public void FindsAllReactiveSystems()
        {
            var updatedFiles = _migration.Migrate(FixturesPath);
            updatedFiles.Length.Should().Be(1);
            updatedFiles.Any(file => file.fileName == Path.Combine(FixturesPath, "AddViewFromObjectPoolSystem.cs")).Should().BeTrue();
        }

        [Fact(Skip = "not finished")]
        public void MigratesToNewApi()
        {
            var updatedFiles = _migration.Migrate(FixturesPath);
            var systemFile = updatedFiles.Single(file => file.fileName == Path.Combine(FixturesPath, "AddViewFromObjectPoolSystem.cs"));

            if (systemFile.fileContent != Expected)
                _output.WriteLine(systemFile.fileContent);

            systemFile.fileContent.Should().Be(Expected);
        }

        const string Expected =
            @"using System.Collections.Generic;
using Entitas;
using UnityEngine;

public sealed class AddViewFromObjectPoolSystem : IInitializeSystem, ReactiveSystem {

    public Collector<Test1Entity> GetTrigger(Context context) {
        return context.CreateCollector(BulletsMatcher.ViewObjectPool, GroupEvent.Removed);
    }



    protected override bool Filter(Entity entity) {

        // TODO Entitas 0.36.0 Migration

        // ensure was: Matcher.AllOf(BulletsMatcher.ViewObjectPool, BulletsMatcher.Position)

        // exclude was: Matcher<Test1Entity>.AnyOf(BulletsMatcher.Destroy, BulletsMatcher.Destroy)

        return ((entitas.hasViewObjectPool && entitas.hasPosition)) && !((entitas.hasDestroy || entitas.hasDestroy));
    }

    Pool _pool;
    Transform _container;

    public void SetPool(Pool pool) {
        _pool = pool;
    }

    public void SetPools(Pools pools)
    {
        _pool = pool;
    }

    public void Initialize() {
    }

    public void Execute(List<Entity> entities) {
    }
}
";
    }
}
