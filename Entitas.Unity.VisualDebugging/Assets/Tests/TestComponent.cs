using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class TestComponent : IComponent {
    public Bounds bounds;
    public Color color;
    public AnimationCurve animationCurve;
    public enum MyEnum {
        Item1,
        Item2,
        Item3
    }
    public MyEnum myEnum;
    public float myFloat;
    public int myInt;
    public Rect rect;
    public string myString;
    public Vector2 vector2;
    public Vector3 vector3;
    public Vector4 vector4;
    public bool myBool;

    public UnityEngine.Object unityObject;
    public GameObject gameObject;
    public MyObject myObject;
    public object anObject;

    public DateTime date;
    public string[] array;
    public int[,] arrayDim2;
    public int[,,] arrayDim3;
    public string[][] jaggedArray;
    public List<string>[] listArray;
    public List<string> list;
}

