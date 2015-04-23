using System;
using Entitas;
using UnityEditor;

public class MyObjectTypeDrawer : ICustomTypeDrawer {
    public bool HandlesType(Type type) {
        return type == typeof(CustomObject);
    }

    public object DrawAndGetNewValue(Entity entity, int index, IComponent component, string fieldName, object value) {
        var myObject = (CustomObject)value;
        if (myObject == null) {
            EditorGUILayout.LabelField(fieldName, "null");
        } else {
            var newValue = EditorGUILayout.TextField(fieldName, myObject.name);
            if (newValue != myObject.name) {
                entity.WillRemoveComponent(index);
                myObject.name = newValue;
                entity.ReplaceComponent(index, component);
            }
        }
        return myObject;
    }
}
