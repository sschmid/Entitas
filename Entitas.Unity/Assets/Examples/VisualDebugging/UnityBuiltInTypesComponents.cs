using Entitas;
using UnityEngine;

[VisualDebugging]
public class BoundsComponent : IComponent {
    public Bounds bounds;
}

[VisualDebugging]
public class ColorComponent : IComponent {
    public Color color;
}

[VisualDebugging]
public class AnimationCurveComponent : IComponent {
    public AnimationCurve animationCurve;
}

[VisualDebugging]
public class MyEnumComponent : IComponent {
    public enum MyEnum {
        Item1,
        Item2,
        Item3
    }

    public MyEnum myEnum;
}

[VisualDebugging]
public class MyFlagsComponent : IComponent {

    [System.Flags]
    public enum MyFlags {
        Item1,
        Item2,
        Item3
    }

    public MyFlags myFlags;
}

[VisualDebugging]
public class MyDoubleComponent : IComponent {
    public double myDouble;
}

[VisualDebugging]
public class MyFloatComponent : IComponent {
    public float myFloat;
}

[VisualDebugging]
public class MyIntComponent : IComponent {
    public int myInt;
}

[VisualDebugging]
public class RectComponent : IComponent {
    public Rect rect;
}

[VisualDebugging]
public class MyStringComponent : IComponent {
    public string myString;
}

[VisualDebugging]
public class Vector2Component : IComponent {
    public Vector2 vector2;
}

[VisualDebugging]
public class Vector3Component : IComponent {
    public Vector3 vector3;
}

[VisualDebugging]
public class Vector4Component : IComponent {
    public Vector4 vector4;
}

[VisualDebugging]
public class MyBoolComponent : IComponent {
    public bool myBool;
}

[VisualDebugging]
public class UnityObjectComponent : IComponent {
    public Object unityObject;
}

[VisualDebugging]
public class GameObjectComponent : IComponent {
    public GameObject gameObject;
}

[VisualDebugging]
public class TextureComponent : IComponent {
    public Texture texture;
}

[VisualDebugging]
public class Texture2DComponent : IComponent {
    public Texture2D texture2D;
}
