using System.Collections.Generic;

public static class ComponentIds {
    public const int AnimationCurve = 0;
    public const int Array2D = 1;
    public const int Array3D = 2;
    public const int Array = 3;
    public const int Bounds = 4;
    public const int Color = 5;
    public const int CustomObject = 6;
    public const int DateTime = 7;
    public const int Dictionary = 8;
    public const int GameObject = 9;
    public const int JaggedArray = 10;
    public const int ListArray = 11;
    public const int List = 12;
    public const int MyBool = 13;
    public const int MyEnum = 14;
    public const int MyFloat = 15;
    public const int MyInt = 16;
    public const int MyString = 17;
    public const int Rect = 18;
    public const int SystemObject = 19;
    public const int UnityObject = 20;
    public const int Vector2 = 21;
    public const int Vector3 = 22;
    public const int Vector4 = 23;

    public const int TotalComponents = 24;

    static readonly Dictionary<int, string> components = new Dictionary<int, string> {
        { 0, "AnimationCurve" },
        { 1, "Array2D" },
        { 2, "Array3D" },
        { 3, "Array" },
        { 4, "Bounds" },
        { 5, "Color" },
        { 6, "CustomObject" },
        { 7, "DateTime" },
        { 8, "Dictionary" },
        { 9, "GameObject" },
        { 10, "JaggedArray" },
        { 11, "ListArray" },
        { 12, "List" },
        { 13, "MyBool" },
        { 14, "MyEnum" },
        { 15, "MyFloat" },
        { 16, "MyInt" },
        { 17, "MyString" },
        { 18, "Rect" },
        { 19, "SystemObject" },
        { 20, "UnityObject" },
        { 21, "Vector2" },
        { 22, "Vector3" },
        { 23, "Vector4" }
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