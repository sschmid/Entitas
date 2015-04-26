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
    public const int DictArray = 8;
    public const int Dictionary = 9;
    public const int GameObject = 10;
    public const int JaggedArray = 11;
    public const int ListArray = 12;
    public const int List = 13;
    public const int MyBool = 14;
    public const int MyEnum = 15;
    public const int MyFloat = 16;
    public const int MyInt = 17;
    public const int MyString = 18;
    public const int Rect = 19;
    public const int SystemObject = 20;
    public const int UnityObject = 21;
    public const int UnsupportedObject = 22;
    public const int Vector2 = 23;
    public const int Vector3 = 24;
    public const int Vector4 = 25;

    public const int TotalComponents = 26;

    static readonly Dictionary<int, string> components = new Dictionary<int, string> {
        { 0, "AnimationCurve" },
        { 1, "Array2D" },
        { 2, "Array3D" },
        { 3, "Array" },
        { 4, "Bounds" },
        { 5, "Color" },
        { 6, "CustomObject" },
        { 7, "DateTime" },
        { 8, "DictArray" },
        { 9, "Dictionary" },
        { 10, "GameObject" },
        { 11, "JaggedArray" },
        { 12, "ListArray" },
        { 13, "List" },
        { 14, "MyBool" },
        { 15, "MyEnum" },
        { 16, "MyFloat" },
        { 17, "MyInt" },
        { 18, "MyString" },
        { 19, "Rect" },
        { 20, "SystemObject" },
        { 21, "UnityObject" },
        { 22, "UnsupportedObject" },
        { 23, "Vector2" },
        { 24, "Vector3" },
        { 25, "Vector4" }
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