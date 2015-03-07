using Entitas;
using Entitas.Unity.VisualDebugging;
using UnityEngine;
using System;

public class GameController : MonoBehaviour {

    void Start() {
        var pool = new DebugPool(ComponentIds.TotalComponents);
        pool.GetGroup(Matcher.Name);
        pool.GetGroup(Matcher.Test);

        for (int i = 0; i < 10; i++) {
            var e = pool.CreateEntity();
            e.AddTest(new Bounds(),
                Color.red,
                AnimationCurve.EaseInOut(0f, 0f, 1f, 1f),
                TestComponent.MyEnum.Item1,
                1.23f,
                42,
                new Rect(),
                "Hello",
                Vector2.one,
                Vector3.one,
                Vector4.one,
                true,
                new MyObject(),
                new object(),
                DateTime.Now
            );
        }
    }
}

