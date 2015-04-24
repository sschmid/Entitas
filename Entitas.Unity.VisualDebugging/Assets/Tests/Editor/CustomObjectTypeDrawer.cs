using System;
using Entitas;
using Entitas.Unity.VisualDebugging;
using UnityEditor;

public class CustomObjectTypeDrawer : ITypeDrawer, IDefaultInstanceCreator {
    public bool HandlesType(Type type) {
        return type == typeof(CustomObject);
    }

    public object CreateDefault(Type type) {
        return new CustomObject("Default");
    }

    public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
        var myObject = (CustomObject)value;
        var newValue = EditorGUILayout.TextField(fieldName, myObject.name);
        if (newValue != myObject.name) {
            entity.WillRemoveComponent(index);
            myObject.name = newValue;
            entity.ReplaceComponent(index, component);
        }
        return myObject;
    }
}
