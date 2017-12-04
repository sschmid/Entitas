using System;
using System.Collections;
using System.Linq;
using DesperateDevs.Unity.Editor;
using DesperateDevs.Utils;
using UnityEditor;

namespace Entitas.VisualDebugging.Unity.Editor {

    public class ListTypeDrawer : ITypeDrawer {

        public bool HandlesType(Type type) {
            return type.GetInterfaces().Contains(typeof(IList));
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            var list = (IList)value;
            var elementType = memberType.GetGenericArguments()[0];
            if (list.Count == 0) {
                list = drawAddElement(list, memberName, elementType);
            } else {
                EditorGUILayout.LabelField(memberName);
            }

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;
            Func<IList> editAction = null;
            for (int i = 0; i < list.Count; i++) {
                var localIndex = i;
                EditorGUILayout.BeginHorizontal();
                {
                    EntityDrawer.DrawObjectMember(elementType, memberName + "[" + localIndex + "]", list[localIndex],
                        target, (newComponent, newValue) => list[localIndex] = newValue);

                    var action = drawEditActions(list, elementType, localIndex);
                    if (action != null) {
                        editAction = action;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if (editAction != null) {
                list = editAction();
            }
            EditorGUI.indentLevel = indent;

            return list;
        }

        static Func<IList> drawEditActions(IList list, Type elementType, int index) {
            if (EditorLayout.MiniButtonLeft("↑")) {
                if (index > 0) {
                    return () => {
                        var otherIndex = index - 1;
                        var other = list[otherIndex];
                        list[otherIndex] = list[index];
                        list[index] = other;
                        return list;
                    };
                }
            }

            if (EditorLayout.MiniButtonMid("↓")) {
                if (index < list.Count - 1) {
                    return () => {
                        var otherIndex = index + 1;
                        var other = list[otherIndex];
                        list[otherIndex] = list[index];
                        list[index] = other;
                        return list;
                    };
                }
            }

            if (EditorLayout.MiniButtonMid("+")) {
                object defaultValue;
                if (EntityDrawer.CreateDefault(elementType, out defaultValue)) {
                    var insertAt = index + 1;
                    return () => {
                        list.Insert(insertAt, defaultValue);
                        return list;
                    };
                }
            }

            if (EditorLayout.MiniButtonRight("-")) {
                var removeAt = index;
                return () => {
                    list.RemoveAt(removeAt);
                    return list;
                };
            }

            return null;
        }

        IList drawAddElement(IList list, string memberName, Type elementType) {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(memberName, "empty");
                if (EditorLayout.MiniButton("add " + elementType.ToCompilableString().ShortTypeName())) {
                    object defaultValue;
                    if (EntityDrawer.CreateDefault(elementType, out defaultValue)) {
                        list.Add(defaultValue);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            return list;
        }
    }
}
