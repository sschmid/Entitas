using System;
using Entitas.VisualDebugging.Unity.Editor;
using UnityEditor;

public class CustomObjectDrawer : ITypeDrawer, IDefaultInstanceCreator
{
    public bool HandlesType(Type type) => type == typeof(MyCustomObject);

    public object CreateDefault(Type type) => new MyCustomObject("Default");

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
    {
        var myObject = (MyCustomObject)value;
        myObject.Name = EditorGUILayout.TextField(memberName, myObject.Name);
        return myObject;
    }
}
