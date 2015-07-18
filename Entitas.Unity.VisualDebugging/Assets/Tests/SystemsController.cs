using Entitas;
using UnityEngine;
using Entitas.Unity.VisualDebugging;

public class SystemsController : MonoBehaviour {
    Systems _systems;

    Pool _pool;

    void Start() {
        _pool = new Pool(ComponentIds.TotalComponents);
        _systems = createSystems();
        _systems.Start();
        _pool.CreateEntity().AddMyString("");
    }

    void Update() {
        _pool.GetGroup(Matcher.MyString).GetSingleEntity().ReplaceMyString("");
        _systems.Execute();
    }

    Systems createSystems() {
        return new DebugSystems()
            .Add(_pool.CreateSlowStartSystem())
            .Add(_pool.CreateSlowStartExecuteSystem())
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
