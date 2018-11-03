using System;
using System.Collections.Generic;
using Entitas;

public class MultiTriggeredMultiReactiveSystemSpy : MultiReactiveSystem<IMyEntity, Contexts> {

    public int didExecute { get { return _didExecute; } }
    public IEntity[] entities { get { return _entities; } }

    public Action<List<IMyEntity>> executeAction;

    protected int _didExecute;
    protected IEntity[] _entities;

    public MultiTriggeredMultiReactiveSystemSpy(Contexts contexts) : base(contexts) {
    }

    protected override ICollector[] GetTrigger(Contexts contexts) {
        return new ICollector[] {
            contexts.test.CreateCollector(TestMatcher.NameAge),
            contexts.test.CreateCollector(TestMatcher.NameAge.Removed())
        };
    }

    protected override bool Filter(IMyEntity entity) {
        return true;
    }

    protected override void Execute(List<IMyEntity> entities) {
        _didExecute += 1;

        if (entities != null) {
            _entities = entities.ToArray();
        } else {
            _entities = null;
        }

        if (executeAction != null) {
            executeAction(entities);
        }
    }
}
