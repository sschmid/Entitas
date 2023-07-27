using Entitas.Unity;
using UnityEngine;
using UnityEditor;

public class ReactiveSystemDestructorController : MonoBehaviour
{
    GameContext _gameContext;
    Game.Entity _initialEntity;

    void Start()
    {
        ContextInitialization.InitializeAllContexts();
        _gameContext = new GameContext();
        _gameContext.CreateContextObserver();
        new TestReactiveSystem(_gameContext);
        _initialEntity = _gameContext.CreateEntity();
        _initialEntity.AddTest();
        _initialEntity.Destroy();
    }

    void Update()
    {
        for (var i = 0; i < 5000; i++)
        {
            var e = _gameContext.CreateEntity();
            if (e == _initialEntity)
            {
                Debug.Log("Success: Reusing entity!");
                EditorApplication.isPlaying = false;
            }
        }
    }
}
