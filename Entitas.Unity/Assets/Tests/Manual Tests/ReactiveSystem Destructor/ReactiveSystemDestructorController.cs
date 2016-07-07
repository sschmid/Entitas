using UnityEngine;
using Entitas;

public class ReactiveSystemDestructorController : MonoBehaviour {
    Entity _initialEntity;

    void Start() {
        var pool = Pools.visualDebugging;
        pool.CreateSystem<TestReactiveSystem>();
        _initialEntity = pool.CreateEntity();
        _initialEntity.isTest = true;
        pool.DestroyEntity(_initialEntity);
    }
	
    void Update() {
        var pool = Pools.visualDebugging;
        for (int i = 0; i < 5000; i++) {
            var e = pool.CreateEntity();
            if (e == _initialEntity) {
                Debug.Log("Reusing entity!");
            }
        }
    }
}
