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
    public const int GameObject = 8;
    public const int JaggedArray = 9;
    public const int ListArray = 10;
    public const int List = 11;
    public const int MyBool = 12;
    public const int MyEnum = 13;
    public const int MyFloat = 14;
    public const int MyInt = 15;
    public const int MyString = 16;
    public const int Rect = 17;
    public const int SystemObject = 18;
    public const int UnityObject = 19;
    public const int Vector2 = 20;
    public const int Vector3 = 21;
    public const int Vector4 = 22;

    public const int TotalComponents = 23;

    static readonly Dictionary<int, string> components = new Dictionary<int, string> {
        { 0, "AnimationCurve" },
        { 1, "Array2D" },
        { 2, "Array3D" },
        { 3, "Array" },
        { 4, "Bounds" },
        { 5, "Color" },
        { 6, "CustomObject" },
        { 7, "DateTime" },
        { 8, "GameObject" },
        { 9, "JaggedArray" },
        { 10, "ListArray" },
        { 11, "List" },
        { 12, "MyBool" },
        { 13, "MyEnum" },
        { 14, "MyFloat" },
        { 15, "MyInt" },
        { 16, "MyString" },
        { 17, "Rect" },
        { 18, "SystemObject" },
        { 19, "UnityObject" },
        { 20, "Vector2" },
        { 21, "Vector3" },
        { 22, "Vector4" }
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