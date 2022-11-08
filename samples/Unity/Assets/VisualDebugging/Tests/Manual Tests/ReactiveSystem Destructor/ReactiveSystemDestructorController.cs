using UnityEngine;
using UnityEditor;

public class ReactiveSystemDestructorController : MonoBehaviour {

    GameEntity _initialEntity;

    void Start() {
        var context = Contexts.sharedInstance.game;
        new TestReactiveSystem(Contexts.sharedInstance);
        _initialEntity = context.CreateEntity();
        _initialEntity.isTest = true;
        _initialEntity.Destroy();
    }

    void Update() {
        var context = Contexts.sharedInstance.game;
        for (int i = 0; i < 5000; i++) {
            var e = context.CreateEntity();
            if (e == _initialEntity) {
                Debug.Log("Success: Reusing entity!");
                EditorApplication.isPlaying = false;
            }
        }
    }
}
