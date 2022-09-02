using System;
using UnityEditor;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity.Editor {

    public class Vector3TypeDrawer : ITypeDrawer {

        public bool HandlesType(Type type) {
            return type == typeof(Vector3);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            return EditorGUILayout.Vector3Field(memberName, (Vector3)value);
        }
    }
}
