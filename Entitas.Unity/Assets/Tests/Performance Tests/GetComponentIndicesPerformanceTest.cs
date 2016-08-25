using Entitas;
using UnityEngine;

public class GetComponentIndicesPerformanceTest : MonoBehaviour {

    Entity _entity;

    void Start() {
        Pools.sharedInstance.visualDebugging = Pools.CreateVisualDebuggingPool();
        _entity = Pools.sharedInstance.visualDebugging.CreateEntity();
    }

    void Update() {
        _entity.AddMyInt(0);
        _entity.RemoveMyInt();

        _entity.GetComponentIndices();
    }
}
