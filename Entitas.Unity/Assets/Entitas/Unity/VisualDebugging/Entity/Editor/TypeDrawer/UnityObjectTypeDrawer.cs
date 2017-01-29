using System;
using Entitas.Api;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {

    public class UnityObjectTypeDrawer : ITypeDrawer {

        public bool HandlesType(Type type) {
            return type == typeof(UnityEngine.Object) ||
                type.IsSubclassOf(typeof(UnityEngine.Object));
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, IEntity entity, int index, IComponent component) {
            return EditorGUILayout.ObjectField(memberName, (UnityEngine.Object)value, memberType, true);
        }
    }
}
