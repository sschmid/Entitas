using Entitas;
using Entitas.Unity;
using UnityEngine;

public class SystemsController : MonoBehaviour
{
    GameContext _gameContext;
    Systems _systems;

    void Start()
    {
        ContextInitialization.InitializeAllContexts();
        _gameContext = new GameContext();
        _gameContext.CreateContextObserver();

        _systems = CreateNestedSystems();
        //// Test calls
        _systems.Initialize();
        _systems.Execute();
        _systems.Cleanup();
        _systems.TearDown();

        _gameContext.CreateEntity().AddMyString("");
    }

    Systems CreateNestedSystems()
    {
        var systems1 = new Feature("Nested 1");
        var systems2 = new Feature("Nested 2");
        var systems3 = new Feature("Nested 3");

        systems1.Add(systems2);
        systems2.Add(systems3);
        systems1.Add(CreateSomeSystems());

        return new Feature("Nested Systems")
            .Add(systems1);
    }

    Systems CreateSomeSystems()
    {
        return new SomeSystems(_gameContext);
    }

    void Update()
    {
        _gameContext.GetGroup(GameMyStringMatcher.MyString).GetSingleEntity()
            .ReplaceMyString(Random.value.ToString());

        _systems.Execute();
        _systems.Cleanup();
    }

    Systems CreateAllSystemCombinations()
    {
        return new Feature("All System Combinations")
            .Add(new SomeInitializeSystem())
            .Add(new SomeExecuteSystem())
            .Add(new SomeReactiveSystem(_gameContext))
            .Add(new SomeInitializeExecuteSystem())
            .Add(new SomeInitializeReactiveSystem(_gameContext));
    }

    Systems CreateSubSystems()
    {
        var allSystems = CreateAllSystemCombinations();
        var subSystems = new Feature("Sub Systems").Add(allSystems);
        return new Feature("Systems with SubSystems")
            .Add(allSystems)
            .Add(allSystems)
            .Add(subSystems)
            .Add(subSystems);
    }

    Systems CreateSameInstance()
    {
        var system = new RandomDurationSystem();
        return new Feature("Same System Instances")
            .Add(system)
            .Add(system)
            .Add(system);
    }

    Systems CreateEmptySystems()
    {
        var systems1 = new Feature("Empty 1");
        var systems2 = new Feature("Empty 2");
        var systems3 = new Feature("Empty 3");

        systems1.Add(systems2);
        systems2.Add(systems3);

        return new Feature("Empty Systems")
            .Add(systems1);
    }

    sealed class SomeSystems : Feature
    {
        public SomeSystems(GameContext gameContext)
        {
            Add(new SlowInitializeSystem());
            Add(new SlowInitializeExecuteSystem());
            Add(new FastSystem());
            Add(new SlowSystem());
            Add(new RandomDurationSystem());
            Add(new AReactiveSystem(gameContext));

            Add(new RandomValueSystem(gameContext));
            Add(new ProcessRandomValueSystem(gameContext));
            Add(new CleanupSystem());
            Add(new TearDownSystem());
            Add(new MixedSystem());
        }
    }
}
