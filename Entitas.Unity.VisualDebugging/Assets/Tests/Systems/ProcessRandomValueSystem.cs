using Entitas;

public class ProcessRandomValueSystem : IReactiveSystem, ISetPool {
    public IMatcher trigger { get { return Matcher.MyFloat; } }

    public GroupEventType eventType { get { return GroupEventType.OnEntityAdded; } }

    Pool _pool;

    public void SetPool(Pool pool) {
        _pool = pool;
    }

    public void Execute(Entity[] entities) {
//        UnityEngine.Debug.Log("entities.Length: " + entities.Length);
        foreach (var e in entities) {
            _pool.DestroyEntity(e);
        }
    }
}

