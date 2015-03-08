using Entitas;
using UnityEngine;
using System;

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
    public MyObject customObject;
    public object myObject;

    public DateTime date;
    public string[] strings;
    public int[] ints;
    public string[][] stringStrings;
}

