using Entitas;
using UnityEngine;
using Entitas.Unity.VisualDebugging;

public class VisualDebuggingExampleSystemsController : MonoBehaviour {

	Pools _pools;
    Systems _systems;

    void Start() {
        _pools = new Pools();
        _pools.visualDebugging = new Pool(
            VisualDebuggingComponentIds.TotalComponents, 0,
            new PoolMetaData(
                "Systems Pool",
                VisualDebuggingComponentIds.componentNames,
                VisualDebuggingComponentIds.componentTypes
            )
        );

        #if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
        new PoolObserver(_pools.visualDebugging);
        #endif

        _systems = createNestedSystems();

        //// Test call
        _systems.Initialize();
        _systems.Execute();
        _systems.Cleanup();
        _systems.TearDown();

        _pools.visualDebugging.CreateEntity().AddMyString("");
    }

    void Update() {
        _pools.visualDebugging.GetGroup(VisualDebuggingMatcher.MyString).GetSingleEntity()
             .ReplaceMyString(Random.value.ToString());

        _systems.Execute();
        _systems.Cleanup();
    }

    Systems createAllSystemCombinations() {
        return new Feature("All System Combinations")
            .Add(new SomeInitializeSystem())
            .Add(new SomeExecuteSystem())
            .Add(new SomeReactiveSystem(_pools))
            .Add(new SomeInitializeExecuteSystem())
            .Add(new SomeInitializeReactiveSystem(_pools));
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
        var system = new RandomDurationSystem();
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
            .Add(new SlowInitializeSystem())
            .Add(new SlowInitializeExecuteSystem())
            .Add(new FastSystem())
            .Add(new SlowSystem())
            .Add(new RandomDurationSystem())
            .Add(new AReactiveSystem(_pools))

            .Add(new RandomValueSystem(_pools))
            .Add(new ProcessRandomValueSystem(_pools))
            .Add(new CleanupSystem())
            .Add(new TearDownSystem())
            .Add(new MixedSystem());
    }
}
