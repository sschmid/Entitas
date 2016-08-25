using Entitas;
using UnityEngine;

public class MatcherPerformanceTest : MonoBehaviour {

    void Update() {
        Matcher
            .AllOf(VisualDebuggingMatcher.MyString)
            .AnyOf(VisualDebuggingMatcher.MyInt, VisualDebuggingMatcher.MyFloat)
            .NoneOf(VisualDebuggingMatcher.MyBool);
    }
}
