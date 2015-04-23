using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Unity.VisualDebugging;
using UnityEngine;

public class GameController : MonoBehaviour {

    void Start() {
        var pool = new DebugPool(ComponentIds.TotalComponents);
        pool.GetGroup(Matcher.Name);
        pool.GetGroup(Matcher.Test);

        for (int i = 0; i < 10; i++) {
            string[][] jaggedArray = new string[2][];
            jaggedArray[0] = new [] { "Entity", "Component", "System" };
            jaggedArray[1] = new [] { "For", "C#" };

            int[,] arrayDim2 = new int[2, 3];
            int[,,] arrayDim3 = new int[3, 2, 4];
            List<string>[] listArray = new List<string>[] {
                new List<string> {"1", "2", "3"},
                new List<string> {"4", "5", "6"}
            };

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
                new UnityEngine.Object(),
                new GameObject("GameObject " + i),
                new MyObject(),
                new object(),
                DateTime.Now,
                new [] { "Entity", "Component", "System" },
                arrayDim2,
                arrayDim3,
                jaggedArray,
                listArray,
                new List<string>{"One", "Two", "Three"}
            );
        }
    }
}

