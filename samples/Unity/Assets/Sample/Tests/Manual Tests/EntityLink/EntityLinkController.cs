using Entitas.Unity;
using UnityEngine;

public class EntityLinkController : MonoBehaviour
{
    void Start()
    {
        ContextInitialization.InitializeAllContexts();
        var gameContext = new GameContext();
        gameContext.CreateContextObserver();
        var entity = gameContext.CreateEntity();

        var go = new GameObject();
        go.Link(entity);

        entity.AddMyGameObject(go);

//        go.Unlink();

        Destroy(go);
    }
}
