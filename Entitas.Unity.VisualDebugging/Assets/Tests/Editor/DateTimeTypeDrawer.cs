using System;
using Entitas;
using UnityEditor;

public class DateTimeTypeDrawer : ICustomTypeDrawer {
    public bool HandlesType(Type type) {
        return type == typeof(DateTime);
    }

    public object DrawAndGetNewValue(Entity entity, int index, IComponent component, string fieldName, object value) {
        return DateTime.Parse(EditorGUILayout.TextField(fieldName, value.ToString()));
    }
}
