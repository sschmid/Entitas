using UnityEngine;
using Entitas;
using Entitas.Unity;
using UnityEditor;

public class CollectorDestructorController : MonoBehaviour
{
    GameContext _gameContext;
    Game.Entity _initialEntity;

    void Start()
    {
        ContextInitialization.InitializeAllContexts();
        _gameContext = new GameContext();
        _gameContext.CreateContextObserver();
        _gameContext.GetGroup(GameTestMatcher.Test).CreateCollector();
        _initialEntity = _gameContext.CreateEntity();
        _initialEntity.AddTest();
        _initialEntity.Destroy();
        // TODO
        // context.ClearGroups();
    }

    void Update()
    {
        for (var i = 0; i < 5000; i++)
        {
            var entity = _gameContext.CreateEntity();
            if (entity == _initialEntity)
            {
                Debug.Log("Reusing entity!");
                EditorApplication.isPlaying = false;
            }
        }
    }
}
