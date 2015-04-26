using System;
using Entitas;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {
    public class DateTimeTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(DateTime);
        }

        public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            return DateTime.Parse(EditorGUILayout.TextField(fieldName, value.ToString()));
        }
    }
}