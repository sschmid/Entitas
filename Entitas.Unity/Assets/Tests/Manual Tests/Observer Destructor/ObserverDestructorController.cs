using UnityEngine;
using Entitas;

public class ObserverDestructorController : MonoBehaviour {
    Entity _initialEntity;

    void Start() {
        var pool = Pools.sharedInstance.visualDebugging = Pools.CreateVisualDebuggingPool();
        pool.GetGroup(VisualDebuggingMatcher.Test).CreateObserver();
        _initialEntity = pool.CreateEntity();
        _initialEntity.isTest = true;
        pool.DestroyEntity(_initialEntity);
        pool.ClearGroups();
    }
	
    void Update() {
        var pool = Pools.sharedInstance.visualDebugging;
        for (int i = 0; i < 5000; i++) {
            var e = pool.CreateEntity();
            if (e == _initialEntity) {
                Debug.Log("Reusing entity!");
            }
        }
    }
}
