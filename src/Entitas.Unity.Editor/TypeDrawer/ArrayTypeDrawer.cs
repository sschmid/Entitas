using System;
using System.Collections;
using DesperateDevs.Extensions;
using DesperateDevs.Unity.Editor;
using UnityEditor;

namespace Entitas.Unity.Editor
{
    public class ArrayTypeDrawer : ITypeDrawer
    {
        public bool HandlesType(Type type) => type.IsArray;

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            var array = (Array)value;
            var elementType = memberType.GetElementType();
            var indent = EditorGUI.indentLevel;

            if (array.Rank == 1) array = DrawRank1(array, memberName, elementType, indent, target);
            else if (array.Rank == 2) array = DrawRank2(array, memberName, elementType, target);
            else if (array.Rank == 3) array = DrawRank3(array, memberName, elementType, target);

            EditorGUI.indentLevel = indent;

            return array;
        }

        /*
         *
         * Rank 1
         *
         */

        Array DrawRank1(Array array, string memberName, Type elementType, int indent, object target)
        {
            var length = array.GetLength(0);
            if (length == 0)
                array = DrawAddElement(array, memberName, elementType);
            else
                EditorGUILayout.LabelField(memberName);

            EditorGUI.indentLevel = indent + 1;

            Func<Array> editAction = null;
            for (var i = 0; i < length; i++)
            {
                var localIndex = i;
                EditorGUILayout.BeginHorizontal();
                {
                    EntityDrawer.DrawObjectMember(elementType, $"{memberName}[{localIndex}]", array.GetValue(localIndex),
                        target, (_, newValue) => array.SetValue(newValue, localIndex));

                    var action = DrawEditActions(array, elementType, localIndex);
                    if (action != null)
                        editAction = action;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (editAction != null)
                array = editAction();

            return array;
        }

        Array DrawAddElement(Array array, string memberName, Type elementType)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(memberName, "empty");
                if (EditorLayout.MiniButton($"add {elementType.ToCompilableString().TypeName()}"))
                {
                    if (EntityDrawer.CreateDefault(elementType, out var defaultValue))
                    {
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

        Array DrawRank2(Array array, string memberName, Type elementType, object target)
        {
            EditorGUILayout.LabelField(memberName);

            for (var i = 0; i < array.GetLength(0); i++)
            {
                var localIndex1 = i;
                for (var j = 0; j < array.GetLength(1); j++)
                {
                    var localIndex2 = j;
                    EntityDrawer.DrawObjectMember(elementType, $"{memberName}[{localIndex1}, {localIndex2}]", array.GetValue(localIndex1, localIndex2),
                        target, (_, newValue) => array.SetValue(newValue, localIndex1, localIndex2));
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

        Array DrawRank3(Array array, string memberName, Type elementType, object target)
        {
            EditorGUILayout.LabelField(memberName);

            for (var i = 0; i < array.GetLength(0); i++)
            {
                var localIndex1 = i;
                for (var j = 0; j < array.GetLength(1); j++)
                {
                    var localIndex2 = j;
                    for (var k = 0; k < array.GetLength(2); k++)
                    {
                        var localIndex3 = k;
                        EntityDrawer.DrawObjectMember(elementType, $"{memberName}[{localIndex1}, {localIndex2}, {localIndex3}]", array.GetValue(localIndex1, localIndex2, localIndex3),
                            target, (_, newValue) => array.SetValue(newValue, localIndex1, localIndex2, localIndex3));
                    }

                    EditorGUILayout.Space();
                }

                EditorGUILayout.Space();
            }

            return array;
        }

        static Func<Array> DrawEditActions(Array array, Type elementType, int index)
        {
            if (EditorLayout.MiniButtonLeft("↑"))
            {
                if (index > 0)
                {
                    return () =>
                    {
                        var otherIndex = index - 1;
                        var other = array.GetValue(otherIndex);
                        array.SetValue(array.GetValue(index), otherIndex);
                        array.SetValue(other, index);
                        return array;
                    };
                }
            }

            if (EditorLayout.MiniButtonMid("↓"))
            {
                if (index < array.Length - 1)
                {
                    return () =>
                    {
                        var otherIndex = index + 1;
                        var other = array.GetValue(otherIndex);
                        array.SetValue(array.GetValue(index), otherIndex);
                        array.SetValue(other, index);
                        return array;
                    };
                }
            }

            if (EditorLayout.MiniButtonMid("+"))
                if (EntityDrawer.CreateDefault(elementType, out var defaultValue))
                    return () => ArrayInsertAt(array, elementType, defaultValue, index + 1);

            if (EditorLayout.MiniButtonRight("-"))
                return () => ArrayRemoveAt(array, elementType, index);

            return null;
        }

        static Array ArrayRemoveAt(Array array, Type elementType, int removeAt)
        {
            var arrayList = new ArrayList(array);
            arrayList.RemoveAt(removeAt);
            return arrayList.ToArray(elementType);
        }

        static Array ArrayInsertAt(Array array, Type elementType, object value, int insertAt)
        {
            var arrayList = new ArrayList(array);
            arrayList.Insert(insertAt, value);
            return arrayList.ToArray(elementType);
        }
    }
}
