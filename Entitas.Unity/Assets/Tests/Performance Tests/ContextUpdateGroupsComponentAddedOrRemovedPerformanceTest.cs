using Entitas;
using UnityEngine;

public class ContextUpdateGroupsComponentAddedOrRemovedPerformanceTest : MonoBehaviour {

    Entity _entity;

    void Start() {
        Contexts.sharedInstance.visualDebugging = Contexts.CreateVisualDebuggingContext();
        Contexts.sharedInstance.visualDebugging.GetGroup(VisualDebuggingMatcher.MyInt);
        _entity = Contexts.sharedInstance.visualDebugging.CreateEntity();
    }

    void Update() {
        _entity.AddMyInt(0);
        _entity.RemoveMyInt();
    }
}
