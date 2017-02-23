using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    public class ListTypeDrawer : ITypeDrawer {

        public bool HandlesType(Type type) {
            return type.GetInterfaces().Contains(typeof(IList));
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, IEntity entity, int index, IComponent component) {
            var list = (IList)value;
            var elementType = memberType.GetGenericArguments()[0];
            if(list.Count == 0) {
                list = drawAddElement(list, memberName, elementType);
            } else {
                EditorGUILayout.LabelField(memberName);
            }

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;
            Action editAction = null;
            for (int i = 0; i < list.Count; i++) {
                EditorGUILayout.BeginHorizontal();
                {
                    EntityDrawer.DrawAndSetElement(elementType, memberName + "[" + i + "]", list[i],
                        entity, index, component, (newComponent, newValue) => list[i] = newValue);

                    if(EntitasEditorLayout.MiniButtonLeft("➚")) {
                        object defaultValue;
                        if(EntityDrawer.CreateDefault(elementType, out defaultValue)) {
                            var insertAt = i;
                            editAction = () => list.Insert(insertAt, defaultValue);
                        }
                    }
                    if(EntitasEditorLayout.MiniButtonMid("➘")) {
                        object defaultValue;
                        if(EntityDrawer.CreateDefault(elementType, out defaultValue)) {
                            var insertAt = i + 1;
                            editAction = () => list.Insert(insertAt, defaultValue);
                        }
                    }
                    if(EntitasEditorLayout.MiniButtonRight("-")) {
                        var removeAt = i;
                        editAction = () => list.RemoveAt(removeAt);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if(editAction != null) {
                editAction();
            }
            EditorGUI.indentLevel = indent;

            return list;
        }

        IList drawAddElement(IList list, string memberName, Type elementType) {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(memberName, "empty");
                if(EntitasEditorLayout.MiniButton("add " + elementType.ToCompilableString().ShortTypeName())) {
                    object defaultValue;
                    if(EntityDrawer.CreateDefault(elementType, out defaultValue)) {
                        list.Add(defaultValue);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            return list;
        }
   }
}
