using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Entitas.Migration.Tests
{
    public class M0360AfterTests
    {
        static string FixturePath => $"{TestExtensions.GetProjectRoot()}/src/Entitas.Migration/fixtures/M0360";

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
            var updatedFiles = _migration.Migrate(FixturePath);
            updatedFiles.Length.Should().Be(1);
            updatedFiles.Any(file => file.fileName == Path.Combine(FixturePath, "AddViewFromObjectPoolSystem.cs")).Should().BeTrue();
        }

        [Fact(Skip = "not finished")]
        public void MigratesToNewApi()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            var systemFile = updatedFiles.Single(file => file.fileName == Path.Combine(FixturePath, "AddViewFromObjectPoolSystem.cs"));

            if (systemFile.fileContent != Expected)
                _output.WriteLine(systemFile.fileContent);

            systemFile.fileContent.Should().Be(Expected);
        }

        const string Expected =
            @"using System.Collections.Generic;
using Entitas;
using UnityEngine;

public sealed class AddViewFromObjectPoolSystem : IInitializeSystem, ReactiveSystem {

    public Collector<TestEntity> GetTrigger(Context context) {
        return context.CreateCollector(BulletsMatcher.ViewObjectPool, GroupEvent.Removed);
    }



    protected override bool Filter(Entity entity) {

        // TODO Entitas 0.36.0 Migration

        // ensure was: Matcher.AllOf(BulletsMatcher.ViewObjectPool, BulletsMatcher.Position)

        // exclude was: Matcher<TestEntity>.AnyOf(BulletsMatcher.Destroy, BulletsMatcher.Destroy)

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
