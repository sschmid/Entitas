using System;
using Entitas;
using UnityEditor;
using UnityEngine;

public class MyObjectTypeDrawer : ICustomTypeDrawer {
    public bool HandlesType(Type type) {
        return type == typeof(MyObject);
    }

    public object DrawAndGetNewValue(Entity entity, int index, IComponent component, string fieldName, object value) {
        var myObject = (MyObject)value;
        var newValue = EditorGUILayout.TextField(fieldName, myObject.name);
        if (newValue != myObject.name) {
            entity.WillRemoveComponent(index);
            myObject.name = newValue;
            entity.ReplaceComponent(index, component);
            Debug.Log("Replaced " + component + "." + fieldName + " = " + newValue);
        }

        return myObject;
    }
}
