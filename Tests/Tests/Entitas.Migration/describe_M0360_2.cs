using System.IO;
using System.Linq;
using Entitas.Migration;
using NSpec;

class describe_M0360_2 : nspec {

    void when_migrating() {

        var dir = TestExtensions.GetProjectRoot() + "/Tests/Tests/Entitas.Migration/Fixtures/M0360";

        IMigration m = null;

        before = () => {
            m = new M0360_2();
        };

        it["finds all reactive system"] = () => {
            var updatedFiles = m.Migrate(dir);
            updatedFiles.Length.should_be(1);
            updatedFiles.Any(file => file.fileName == Path.Combine(dir, "AddViewFromObjectPoolSystem.cs")).should_be_true();
        };

        it["migrates to new api"] = () => {
            var updatedFiles = m.Migrate(dir);
            var systemFile = updatedFiles.Single(file => file.fileName == Path.Combine(dir, "AddViewFromObjectPoolSystem.cs"));

            const string expected =
@"using System.Collections.Generic;
using Entitas;
using UnityEngine;

public sealed class AddViewFromObjectPoolSystem : IInitializeSystem, ReactiveSystem {

    public Collector GetTrigger(Context context) {
        return context.CreateCollector(BulletsMatcher.ViewObjectPool, GroupEvent.Removed);
    }



    protected override bool Filter(Entity entity) {

        // TODO Entitas 0.36.0 Migration

        // ensure was: Matcher.AllOf(BulletsMatcher.ViewObjectPool, BulletsMatcher.Position)

        // exclude was: Matcher.AnyOf(BulletsMatcher.Destroy, BulletsMatcher.Destroy)

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
            //if(systemFile.fileContent != expected) {
                //System.Console.WriteLine(systemFile.fileContent);
            //}

            //systemFile.fileContent.should_be(expected);
        };
    }
}
