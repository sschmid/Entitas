using Entitas;
using UnityEngine;
using Entitas.Unity.VisualDebugging;

public class SystemsController : MonoBehaviour {
    Systems _systems;

    Pool _pool;

    void Start() {
        _pool = new Pool(ComponentIds.TotalComponents);
        new PoolObserver(_pool, "Systems Pool");
        _systems = createSystems();
        _systems.Initialize();
        _pool.CreateEntity().AddMyString("");
    }

    void Update() {
        _pool.GetGroup(Matcher.MyString).GetSingleEntity().ReplaceMyString("");
        _systems.Execute();
    }

    Systems createSystems() {
        return new DebugSystems()
            .Add(_pool.CreateSlowInitializeSystem())
            .Add(_pool.CreateSlowInitializeExecuteSystem())
            .Add(_pool.CreateFastSystem())
            .Add(_pool.CreateSlowSystem())
            .Add(_pool.CreateRandomDurationSystem())
            .Add(_pool.CreateAReactiveSystem())
            .Add(_pool.CreateRandomValueSystem())
            .Add(_pool.CreateProcessRandomValueSystem())
            .Add(createSubSystems());
    }

    Systems createSubSystems() {
        return new DebugSystems("Sub Systems")
            .Add(_pool.CreateFastSystem())
            .Add(_pool.CreateSlowSystem())
            .Add(createSubSubSystems());
    }

    Systems createSubSubSystems() {
        return new DebugSystems("Sub Sub Systems")
            .Add(_pool.CreateRandomDurationSystem())
            .Add(_pool.CreateAReactiveSystem());
    }
}
