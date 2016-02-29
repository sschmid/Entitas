using UnityEngine;
using Entitas;

public class ObserverDestructorController : MonoBehaviour {
    Entity _initialEntity;

    void Start() {
        Pools.pool.GetGroup(Matcher.Test).CreateObserver();
        _initialEntity = Pools.pool.CreateEntity();
        _initialEntity.isTest = true;
        Pools.pool.DestroyEntity(_initialEntity);
        Pools.pool.ClearGroups();
    }
	
    void Update() {
        for (int i = 0; i < 5000; i++) {
            var e = Pools.pool.CreateEntity();
            if (e == _initialEntity) {
                Debug.Log("Reusing entity!");
            }
        }
    }
}
