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
            .Add(_pool.CreateSystem<SlowStartSystem>())
            .Add(_pool.CreateSystem<SlowStartExecuteSystem>())
            .Add(_pool.CreateSystem<FastSystem>())
            .Add(_pool.CreateSystem<SlowSystem>())
            .Add(_pool.CreateSystem<RandomDurationSystem>())
            .Add(_pool.CreateSystem<AReactiveSystem>());
    }
}
