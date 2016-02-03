using Entitas;
using UnityEngine;
using Entitas.Unity.VisualDebugging;

public class SystemsController : MonoBehaviour {
    Systems _systems;

    Pool _pool;

    void Start() {
        _pool = new Pool(ComponentIds.TotalComponents, 0, new PoolMetaData("Systems Pool", ComponentIds.componentNames));
        new PoolObserver(_pool, ComponentIds.componentTypes);
        _systems = createNestedSystems();
        _systems.Initialize();
        _pool.CreateEntity().AddMyString("");
    }

    void Update() {
        _pool.GetGroup(Matcher.MyString).GetSingleEntity().ReplaceMyString("");
        _systems.Execute();
    }

    Systems createAllSystemCombinations() {
        return new DebugSystems("All System Combinations")
            .Add(_pool.CreateSystem<SomeInitializeSystem>())
            .Add(_pool.CreateSystem<SomeExecuteSystem>())
            .Add(_pool.CreateSystem<SomeReactiveSystem>())
            .Add(_pool.CreateSystem<SomeInitializeExecuteSystem>())
            .Add(_pool.CreateSystem<SomeInitializeReactiveSystem>());
    }

    Systems createSubSystems() {
        var allSystems = createAllSystemCombinations();
        var subSystems = new DebugSystems("Sub Systems").Add(allSystems);
        
        return new DebugSystems("Systems with SubSystems")
            .Add(allSystems)
            .Add(allSystems)
            .Add(subSystems)
            .Add(subSystems);
    }

    Systems createSameInstance() {
        var system = _pool.CreateSystem<RandomDurationSystem>();
        return new DebugSystems("Same System Instances")
            .Add(system)
            .Add(system)
            .Add(system);
    }

    Systems createNestedSystems() {
        var systems1 = new DebugSystems("Nested 1");
        var systems2 = new DebugSystems("Nested 2");
        var systems3 = new DebugSystems("Nested 3");

        systems1.Add(systems2);
        systems2.Add(systems3);
        systems1.Add(createSomeSystems());

        return new DebugSystems("Nested Systems")
            .Add(systems1);
    }

    Systems createEmptySystems() {
        var systems1 = new DebugSystems("Empty 1");
        var systems2 = new DebugSystems("Empty 2");
        var systems3 = new DebugSystems("Empty 3");

        systems1.Add(systems2);
        systems2.Add(systems3);

        return new DebugSystems("Empty Systems")
            .Add(systems1);
    }

    Systems createSomeSystems() {
        return new DebugSystems("Some Systems")
            .Add(_pool.CreateSystem<SlowInitializeSystem>())
            .Add(_pool.CreateSystem<SlowInitializeExecuteSystem>())
            .Add(_pool.CreateSystem<FastSystem>())
            .Add(_pool.CreateSystem<SlowSystem>())
            .Add(_pool.CreateSystem<RandomDurationSystem>())
            .Add(_pool.CreateSystem<AReactiveSystem>())
            .Add(_pool.CreateSystem<RandomValueSystem>())
            .Add(_pool.CreateSystem<ProcessRandomValueSystem>());
    }
}
