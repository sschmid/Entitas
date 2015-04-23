using Entitas;
using UnityEngine;

public class BoundsComponent : IComponent {
    public Bounds bounds;
}

public class ColorComponent : IComponent {
    public Color color;
}

public class AnimationCurveComponent : IComponent {
    public AnimationCurve animationCurve;
}

public class MyEnumComponent : IComponent {
    public enum MyEnum {
        Item1,
        Item2,
        Item3
    }

    public MyEnum myEnum;
}

public class MyFloatComponent : IComponent {
    public float myFloat;
}

public class MyIntComponent : IComponent {
    public int myInt;
}

public class RectComponent : IComponent {
    public Rect rect;
}

public class MyStringComponent : IComponent {
    public string myString;
}

public class Vector2Component : IComponent {
    public Vector2 vector2;
}

public class Vector3Component : IComponent {
    public Vector3 vector3;
}

public class Vector4Component : IComponent {
    public Vector4 vector4;
}

public class MyBoolComponent : IComponent {
    public bool myBool;
}

public class UnityObjectComponent : IComponent {
    public Object unityObject;
}

public class GameObjectComponent : IComponent {
    public GameObject gameObject;
}


