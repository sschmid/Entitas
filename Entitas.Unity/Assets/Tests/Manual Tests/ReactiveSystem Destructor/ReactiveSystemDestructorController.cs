using UnityEngine;
using Entitas;
using UnityEditor;

public class ReactiveSystemDestructorController : MonoBehaviour {

    Entity _initialEntity;

    void Start() {
        var pool = Pools.sharedInstance.visualDebugging = Pools.CreateVisualDebuggingPool();
        pool.CreateSystem(new TestReactiveSystem());
        _initialEntity = pool.CreateEntity();
        _initialEntity.isTest = true;
        pool.DestroyEntity(_initialEntity);
    }
	
    void Update() {
        var pool = Pools.sharedInstance.visualDebugging;
        for (int i = 0; i < 5000; i++) {
            var e = pool.CreateEntity();
            if(e == _initialEntity) {
                Debug.Log("Success: Reusing entity!");
                EditorApplication.isPlaying = false;
            }
        }
    }
}
