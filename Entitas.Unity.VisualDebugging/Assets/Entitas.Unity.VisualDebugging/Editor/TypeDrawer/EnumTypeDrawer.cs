using System;
using Entitas;
using Entitas.Unity.VisualDebugging;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {
    public class EnumTypeDrawer : ITypeDrawer {
        public bool HandlesType(Type type) {
            return type.IsEnum;
        }

        public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            return EditorGUILayout.EnumPopup(fieldName, (Enum)value);
        }
    }
}