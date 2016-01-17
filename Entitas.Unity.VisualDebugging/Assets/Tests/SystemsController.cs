using Entitas;
using UnityEngine;
using Entitas.Unity.VisualDebugging;

public class SystemsController : MonoBehaviour {
    Systems _systems;

    Pool _pool;

    void Start() {
        _pool = new Pool(ComponentIds.TotalComponents, 0, new PoolMetaData("Systems Pool", ComponentIds.componentNames));
        new PoolObserver(_pool, ComponentIds.componentTypes);
        _systems = createSubSystems();
        _systems.Initialize();
        _pool.CreateEntity().AddMyString("");
    }

    void Update() {
        _pool.GetGroup(Matcher.MyString).GetSingleEntity().ReplaceMyString("");
        _systems.Execute();
    }

    Systems createAllSystemCombinations() {
        return new DebugSystems("All System Combinations")
            .Add(_pool.CreateSomeInitializeSystem())
            .Add(_pool.CreateSomeExecuteSystem())
            .Add(_pool.CreateSomeReactiveSystem())
            .Add(_pool.CreateSomeInitializeExecuteSystem())
            .Add(_pool.CreateSomeInitializeReactiveSystem());
    }

    Systems createSubSystems() {
        var allSystems = createAllSystemCombinations();
        var subSystems = new DebugSystems("Sub Systems").Add(allSystems);
        
        return new DebugSystems("Systems with SubSystems")
            .Add(allSystems)
            .Add(allSystems)
            .Add(subSystems);
    }

    Systems createSameInstance() {
        var system = _pool.CreateSystem<RandomDurationSystem>();
        return new DebugSystems("Same System Instances")
            .Add(system)
            .Add(system)
            .Add(system);
    }

    Systems createSystems() {
        return new DebugSystems("Some Systems")
            .Add(_pool.CreateSlowInitializeSystem())
            .Add(_pool.CreateSlowInitializeExecuteSystem())
            .Add(_pool.CreateFastSystem())
            .Add(_pool.CreateSlowSystem())
            .Add(_pool.CreateRandomDurationSystem())
            .Add(_pool.CreateAReactiveSystem())
            .Add(_pool.CreateRandomValueSystem())
            .Add(_pool.CreateProcessRandomValueSystem());
    }
}
