using Entitas.Unity;
using UnityEngine;

public class EntityStressTestController : MonoBehaviour
{
    public int count;

    GameContext _gameContext;
    bool _flag;

    void Start()
    {
        ContextInitialization.InitializeAllContexts();
        _gameContext = new GameContext();
        _gameContext.CreateContextObserver();

        // for (var i = 0; i < count; i++)
        // {
        //     var e = _contexts.game.CreateEntity();
        //     e.AddMyInt(i);
        //     e.AddMyString(i.ToString());
        // }
    }

    void Update()
    {
        // foreach (var e in _contexts.game.GetEntities())
        //     e.ReplaceMyInt(e.myInt.Value + 1);

        if (Time.frameCount % 60 == 0)
        {
            _flag = !_flag;
            if (_flag)
                for (var i = 0; i < count; i++)
                    _gameContext.CreateEntity().AddMyInt(i);
            else
                foreach (var entity in _gameContext.GetEntities())
                    entity.Destroy();
        }
    }
}
