using System;
using Entitas;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {
    public class DateTimeTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type == typeof(DateTime);
        }

        // Note: This is a very basic implementation. The ToString() method conversion will cut off milliseconds.
        public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            var dateString = value.ToString();
            var newDateString = EditorGUILayout.TextField(fieldName, dateString);

            return newDateString != dateString
                        ? DateTime.Parse(newDateString)
                        : value;
        }
    }
}