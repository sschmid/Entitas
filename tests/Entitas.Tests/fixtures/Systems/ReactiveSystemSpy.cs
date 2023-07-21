using System;
using System.Collections.Generic;
using Entitas;

public interface IReactiveSystemSpy
{
    int DidInitialize { get; }
    int DidExecute { get; }
    int DidCleanup { get; }
    int DidTearDown { get; }
    TestEntity[] Entities { get; }
}

public class ReactiveSystemSpy : ReactiveSystem<TestEntity>, IReactiveSystemSpy, IInitializeSystem, ICleanupSystem, ITearDownSystem
{
    public int DidInitialize => _didInitialize;
    public int DidExecute => _didExecute;
    public int DidCleanup => _didCleanup;
    public int DidTearDown => _didTearDown;
    public TestEntity[] Entities => _entities;

    public Action<List<TestEntity>> ExecuteAction;

    protected int _didInitialize;
    protected int _didExecute;
    protected int _didCleanup;
    protected int _didTearDown;
    protected TestEntity[] _entities;

    readonly Func<TestEntity, bool> _filter;

    public ReactiveSystemSpy(ICollector<TestEntity> collector) : base(collector) { }

    public ReactiveSystemSpy(ICollector<TestEntity> collector, Func<IEntity, bool> filter) : this(collector)
    {
        _filter = filter;
    }

    protected override ICollector<TestEntity> GetTrigger(IContext<TestEntity> context) => null;

    protected override bool Filter(TestEntity entity) => _filter?.Invoke(entity) != false;

    public void Initialize() => _didInitialize += 1;

    protected override void Execute(List<TestEntity> entities)
    {
        _didExecute += 1;
        _entities = entities?.ToArray();
        ExecuteAction?.Invoke(entities);
    }

    public void Cleanup() => _didCleanup += 1;

    public void TearDown() => _didTearDown += 1;
}
