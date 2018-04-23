using Entitas;
using Entitas.VisualDebugging.Unity;
using UnityEngine;

[Game]
public class BoundsComponent : IComponent {
    public Bounds bounds;
}

[Game]
public class ColorComponent : IComponent {
    public Color color;
}

[Game]
public class AnimationCurveComponent : IComponent {
    public AnimationCurve animationCurve;
}

[Game]
public class MyEnumComponent : IComponent {

    public enum MyEnum {
        Item1,
        Item2,
        Item3
    }

    public MyEnum myEnum;
}

[Game]
public class MyFlagsComponent : IComponent {

    [System.Flags]
    public enum MyFlags {
        Item1 = 1,
        Item2 = 2,
        Item3 = 4,
        Item4 = 8
    }

    public MyFlags myFlags;
}

[Game]
public class MyDoubleComponent : IComponent {
    public double myDouble;
}

[Game]
public class MyFloatComponent : IComponent {
    public float myFloat;
}

[Game]
public class MyIntComponent : IComponent {
    public int myInt;
}

[Game, DontDrawComponent]
public class MyHiddenIntComponent : IComponent {
    public int myInt;
}

[Game]
public class RectComponent : IComponent {
    public Rect rect;
}

[Game, Input]
public class MyStringComponent : IComponent {
    public string myString;

    public override string ToString() {
        return "MyString(" + myString + ")";
    }
}

[Game]
public class Vector2Component : IComponent {
    public Vector2 vector2;
}

[Game]
public class Vector3Component : IComponent {
    public Vector3 vector3;
}

[Game]
public class Vector4Component : IComponent {
    public Vector4 vector4;
}

[Game]
public class MyBoolComponent : IComponent {
    public bool myBool;
}

[Game]
public class UnityObjectComponent : IComponent {
    public Object unityObject;
}

[Game]
public class GameObjectComponent : IComponent {
    public GameObject gameObject;
}

[Game]
public class TextureComponent : IComponent {
    public Texture texture;
}

[Game]
public class Texture2DComponent : IComponent {
    public Texture2D texture2D;
}
