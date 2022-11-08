using System.Collections.Generic;
using Entitas;
using UnityEngine;

public sealed class AddViewFromObjectPoolSystem : ISetPool, IReactiveSystem, IEnsureComponents, IExcludeComponents {

    public TriggerOnEvent trigger {
        get { return BulletsMatcher.ViewObjectPool.OnEntityRemoved(); } }

    public IMatcher ensureComponents { get { return Matcher.AllOf(BulletsMatcher.ViewObjectPool, BulletsMatcher.Position); } }

    public IMatcher excludeComponents {
        get
        {
            return Matcher<TestEntity>.AnyOf(BulletsMatcher.Destroy, BulletsMatcher.Destroy);
        }
    }

    Pool _pool;
    Transform _container;

    public void SetPool(Context pool) {
        _pool = pool;
    }

    public void SetPools(Contexts pools)
    {
        _pools = pools;
    }

    public void Initialize() {
    }

    public void Execute(List<Entity> entities) {
    }
}


    public sealed class MoveSystem : ISetPool, IExecuteSystem {

    Group _group;

    public void SetPool(Context pool) {
        _group = pool.GetGroup(Matcher.AllOf(GameMatcher.Move, GameMatcher.Position));
    }

    public void Execute() {
        foreach (var e in _group.GetEntities()) {
            var move = e.move;
            var pos = e.position;
            e.ReplacePosition(pos.x, pos.y + move.speed, pos.z);
        }
    }
}
