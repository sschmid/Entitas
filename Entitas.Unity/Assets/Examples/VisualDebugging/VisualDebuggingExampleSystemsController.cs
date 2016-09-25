using Entitas;
using UnityEngine;
using Entitas.Unity.VisualDebugging;

public class VisualDebuggingExampleSystemsController : MonoBehaviour {

    Systems _systems;

    Pool _pool;

    void Start() {
        _pool = new Pool(VisualDebuggingComponentIds.TotalComponents, 0, new PoolMetaData("Systems Pool", VisualDebuggingComponentIds.componentNames, VisualDebuggingComponentIds.componentTypes));
        #if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
        new PoolObserver(_pool);
        #endif
        _systems = createNestedSystems();
        _systems.Initialize();
        _pool.CreateEntity().AddMyString("");
    }

    void Update() {
        _pool.GetGroup(VisualDebuggingMatcher.MyString).GetSingleEntity().ReplaceMyString(Random.value.ToString());
        _systems.Execute();
    }

    Systems createAllSystemCombinations() {
        return new Feature("All System Combinations")
            .Add(_pool.CreateSystem(new SomeInitializeSystem()))
            .Add(_pool.CreateSystem(new SomeExecuteSystem()))
            .Add(_pool.CreateSystem(new SomeReactiveSystem()))
            .Add(_pool.CreateSystem(new SomeInitializeExecuteSystem()))
            .Add(_pool.CreateSystem(new SomeInitializeReactiveSystem()));
    }

    Systems createSubSystems() {
        var allSystems = createAllSystemCombinations();
        var subSystems = new Feature("Sub Systems").Add(allSystems);
        
        return new Feature("Systems with SubSystems")
            .Add(allSystems)
            .Add(allSystems)
            .Add(subSystems)
            .Add(subSystems);
    }

    Systems createSameInstance() {
        var system = _pool.CreateSystem(new RandomDurationSystem());
        return new Feature("Same System Instances")
            .Add(system)
            .Add(system)
            .Add(system);
    }

    Systems createNestedSystems() {
        var systems1 = new Feature("Nested 1");
        var systems2 = new Feature("Nested 2");
        var systems3 = new Feature("Nested 3");

        systems1.Add(systems2);
        systems2.Add(systems3);
        systems1.Add(createSomeSystems());

        return new Feature("Nested Systems")
            .Add(systems1);
    }

    Systems createEmptySystems() {
        var systems1 = new Feature("Empty 1");
        var systems2 = new Feature("Empty 2");
        var systems3 = new Feature("Empty 3");

        systems1.Add(systems2);
        systems2.Add(systems3);

        return new Feature("Empty Systems")
            .Add(systems1);
    }

    Systems createSomeSystems() {
        return new Feature("Some Systems")
            .Add(_pool.CreateSystem(new SlowInitializeSystem()))
            .Add(_pool.CreateSystem(new SlowInitializeExecuteSystem()))
            .Add(_pool.CreateSystem(new FastSystem()))
            .Add(_pool.CreateSystem(new SlowSystem()))
            .Add(_pool.CreateSystem(new RandomDurationSystem()))
            .Add(_pool.CreateSystem(new AReactiveSystem()))
            .Add(_pool.CreateSystem(new RandomValueSystem()))
            .Add(_pool.CreateSystem(new ProcessRandomValueSystem()));
    }
}
