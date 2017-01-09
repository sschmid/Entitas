using Entitas;
using UnityEngine;

public class EntityGetComponentIndicesPerformanceTest : MonoBehaviour {

    Entity _entity;

    void Start() {
        Contexts.sharedInstance.visualDebugging = Contexts.CreateVisualDebuggingContext();
        _entity = Contexts.sharedInstance.visualDebugging.CreateEntity();
    }

    void Update() {
        _entity.AddMyInt(0);
        _entity.RemoveMyInt();

        _entity.GetComponentIndices();
    }
}
