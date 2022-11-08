using UnityEngine;
using Entitas;
using UnityEditor;

public class CollectorDestructorController : MonoBehaviour {

    GameEntity _initialEntity;

    void Start() {
        var context = Contexts.sharedInstance.game;
        context.GetGroup(GameMatcher.Test).CreateCollector();
        _initialEntity = context.CreateEntity();
        _initialEntity.isTest = true;
        _initialEntity.Destroy();
        // TODO
        //context.ClearGroups();
    }

    void Update() {
        var context = Contexts.sharedInstance.game;
        for (int i = 0; i < 5000; i++) {
            var e = context.CreateEntity();
            if (e == _initialEntity) {
                Debug.Log("Reusing entity!");
                EditorApplication.isPlaying = false;
            }
        }
    }
}
