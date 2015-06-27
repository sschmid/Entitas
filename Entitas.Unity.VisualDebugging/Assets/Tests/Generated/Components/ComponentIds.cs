public static class ComponentIds {
    public const int AnimationCurve = 0;
    public const int Array2D = 1;
    public const int Array3D = 2;
    public const int Array = 3;
    public const int Bounds = 4;
    public const int Color = 5;
    public const int CustomObject = 6;
    public const int DateTime = 7;
    public const int DictArray = 8;
    public const int Dictionary = 9;
    public const int GameObject = 10;
    public const int HashSet = 11;
    public const int JaggedArray = 12;
    public const int ListArray = 13;
    public const int List = 14;
    public const int MyBool = 15;
    public const int MyEnum = 16;
    public const int MyFloat = 17;
    public const int MyInt = 18;
    public const int MyString = 19;
    public const int Rect = 20;
    public const int SystemObject = 21;
    public const int UnityObject = 22;
    public const int UnsupportedObject = 23;
    public const int Vector2 = 24;
    public const int Vector3 = 25;
    public const int Vector4 = 26;

    public const int TotalComponents = 27;

    static readonly string[] components = {
        "AnimationCurve",
        "Array2D",
        "Array3D",
        "Array",
        "Bounds",
        "Color",
        "CustomObject",
        "DateTime",
        "DictArray",
        "Dictionary",
        "GameObject",
        "HashSet",
        "JaggedArray",
        "ListArray",
        "List",
        "MyBool",
        "MyEnum",
        "MyFloat",
        "MyInt",
        "MyString",
        "Rect",
        "SystemObject",
        "UnityObject",
        "UnsupportedObject",
        "Vector2",
        "Vector3",
        "Vector4"
    };

    public static string IdToString(int componentId) {
        return components[componentId];
    }
}

namespace Entitas {
    public partial class Matcher : AllOfMatcher {
        public Matcher(int index) : base(new [] { index }) {
        }

        public override string ToString() {
            return ComponentIds.IdToString(indices[0]);
        }
    }
}