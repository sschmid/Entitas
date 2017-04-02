using System;
using Entitas.VisualDebugging.Unity.Editor;
using UnityEditor;

public class CustomObjectDrawer : ITypeDrawer, IDefaultInstanceCreator {

    public bool HandlesType(Type type) {
        return type == typeof(CustomObject);
    }

    public object CreateDefault(Type type) {
        return new CustomObject("Default");
    }

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
        var myObject = (CustomObject)value;
        myObject.name = EditorGUILayout.TextField(memberName, myObject.name);
        return myObject;
    }
}
