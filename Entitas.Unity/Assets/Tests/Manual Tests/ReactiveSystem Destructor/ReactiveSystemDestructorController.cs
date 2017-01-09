using UnityEngine;
using Entitas;
using UnityEditor;

public class ReactiveSystemDestructorController : MonoBehaviour {

    Entity _initialEntity;

    void Start() {
        var context = Contexts.sharedInstance.visualDebugging = Contexts.CreateVisualDebuggingContext();
        new TestReactiveSystem(Contexts.sharedInstance);
        _initialEntity = context.CreateEntity();
        _initialEntity.isTest = true;
        context.DestroyEntity(_initialEntity);
    }
	
    void Update() {
        var context = Contexts.sharedInstance.visualDebugging;
        for (int i = 0; i < 5000; i++) {
            var e = context.CreateEntity();
            if(e == _initialEntity) {
                Debug.Log("Success: Reusing entity!");
                EditorApplication.isPlaying = false;
            }
        }
    }
}
