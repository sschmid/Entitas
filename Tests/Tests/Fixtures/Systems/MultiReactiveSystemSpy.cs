using System;
using System.Collections.Generic;
using Entitas;

public interface INameAgeEntity : IEntity, INameAge {
}

public class MultiReactiveSystemSpy : MultiReactiveSystem<INameAgeEntity, Contexts> {

    public int didExecute { get { return _didExecute; } }
    public IEntity[] entities { get { return _entities; } }

    public Action<List<INameAgeEntity>> executeAction;

    protected int _didExecute;
    protected IEntity[] _entities;

    public MultiReactiveSystemSpy(Contexts contexts) : base(contexts) {
    }

    protected override ICollector[] GetTrigger(Contexts contexts) {
        return new ICollector[] {
            contexts.test.CreateCollector(TestMatcher.NameAge),
            contexts.test2.CreateCollector(Test2Matcher.NameAge)
        };
    }

    protected override bool Filter(INameAgeEntity entity) {
        return true;
    }

    protected override void Execute(List<INameAgeEntity> entities) {
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
