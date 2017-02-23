using System;
using UnityEditor;
using System.Collections;

namespace Entitas.Unity.VisualDebugging {

    public class ArrayTypeDrawer : ITypeDrawer {

        public bool HandlesType(Type type) {
            return type.IsArray;
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, IEntity entity, int index, IComponent component) {
            var array = (Array)value;
            var elementType = memberType.GetElementType();
            var indent = EditorGUI.indentLevel;

            if(array.Rank == 1) {
                array = drawRank1(array, memberName, elementType, indent, entity, index, component);
            } else if(array.Rank == 2) {
                array = drawRank2(array, memberName, elementType, entity, index, component);
            } else if(array.Rank == 3) {
                array = drawRank3(array, memberName, elementType, entity, index, component);
            }

            EditorGUI.indentLevel = indent;

            return array;
        }

        /*
         *
         * Rank 1
         *
         */

        Array drawRank1(Array array, string memberName, Type elementType, int indent, IEntity entity, int index, IComponent component) {
            var length = array.GetLength(0);
            if(length == 0) {
                array = drawAddElement(array, memberName, elementType);
            } else {
                EditorGUILayout.LabelField(memberName);
            }

            EditorGUI.indentLevel = indent + 1;

            Func<Array> editAction = null;
            for (int i = 0; i < length; i++) {
                var localIndex = i;
                EditorGUILayout.BeginHorizontal();
                {
                    EntityDrawer.DrawAndSetElement(elementType, memberName + "[" + localIndex + "]", array.GetValue(localIndex),
                                                       entity, index, component, (newComponent, newValue) => array.SetValue(newValue, localIndex));

                    var action = drawEditActions(array, elementType, localIndex);
                    if(action != null) {
                        editAction = action;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if(editAction != null) {
                array = editAction();
            }

            return array;
        }

        Array drawAddElement(Array array, string memberName, Type elementType) {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(memberName, "empty");
                if(EntitasEditorLayout.MiniButton("add " + elementType.ToCompilableString().ShortTypeName())) {
                    object defaultValue;
                    if(EntityDrawer.CreateDefault(elementType, out defaultValue)) {
                        var newArray = Array.CreateInstance(elementType, 1);
                        newArray.SetValue(defaultValue, 0);
                        array = newArray;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            return array;
        }

        /*
         *
         * Rank 2
         *
         */

        Array drawRank2(Array array, string memberName, Type elementType, IEntity entity, int index, IComponent component) {
            EditorGUILayout.LabelField(memberName);

            for (int i = 0; i < array.GetLength(0); i++) {
                var localIndex1 = i;
                for (int j = 0; j < array.GetLength(1); j++) {
                    var localIndex2 = j;
                    EntityDrawer.DrawAndSetElement(elementType, memberName + "[" + localIndex1 + ", " + localIndex2 + "]", array.GetValue(localIndex1, localIndex2),
                                                       entity, index, component, (newComponent, newValue) => array.SetValue(newValue, localIndex1, localIndex2));
                }
                EditorGUILayout.Space();
            }

            return array;
        }

        /*
         *
         * Rank 3
         *
         */

        Array drawRank3(Array array, string memberName, Type elementType, IEntity entity, int index, IComponent component) {
            EditorGUILayout.LabelField(memberName);

            for (int i = 0; i < array.GetLength(0); i++) {
                var localIndex1 = i;
                for (int j = 0; j < array.GetLength(1); j++) {
                    var localIndex2 = j;
                    for (int k = 0; k < array.GetLength(2); k++) {
                        var localIndex3 = k;
                        EntityDrawer.DrawAndSetElement(elementType, memberName + "[" + localIndex1 + ", " + localIndex2 + " ," + localIndex3 + "]", array.GetValue(localIndex1, localIndex2, localIndex3),
                                                           entity, index, component, (newComponent, newValue) => array.SetValue(newValue, localIndex1, localIndex2, localIndex3));
                    }
                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
            }

            return array;
        }

        static Func<Array> drawEditActions(Array array, Type elementType, int index) {
            if(EntitasEditorLayout.MiniButtonLeft("➚")) {
                object defaultValue;
                if(EntityDrawer.CreateDefault(elementType, out defaultValue)) {
                    return () => arrayInsertAt(array, elementType, defaultValue, index);
                }
            }
            if(EntitasEditorLayout.MiniButtonMid("➘")) {
                object defaultValue;
                if(EntityDrawer.CreateDefault(elementType, out defaultValue)) {
                    return () => arrayInsertAt(array, elementType, defaultValue, index + 1);
                }
            }
            if(EntitasEditorLayout.MiniButtonRight("-")) {
                return () => arrayRemoveAt(array, elementType, index);
            }

            return null;
        }

        static Array arrayRemoveAt(Array array, Type elementType, int removeAt) {
            var arrayList = new ArrayList(array);
            arrayList.RemoveAt(removeAt);
            return arrayList.ToArray(elementType);
        }

        static Array arrayInsertAt(Array array, Type elementType, object value, int insertAt) {
            var arrayList = new ArrayList(array);
            arrayList.Insert(insertAt, value);
            return arrayList.ToArray(elementType);
        }
    }
}
