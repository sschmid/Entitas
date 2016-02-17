using UnityEngine;
using Entitas;

public class ReactiveSystemDestructorController : MonoBehaviour {
    Entity _initialEntity;

    void Start() {
        Pools.pool.CreateSystem<TestReactiveSystem>();
        _initialEntity = Pools.pool.CreateEntity();
        _initialEntity.isTest = true;
        Pools.pool.DestroyEntity(_initialEntity);
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
