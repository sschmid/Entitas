using System;
using System.Collections;
using System.Collections.Generic;
using DesperateDevs.Extensions;
using DesperateDevs.Unity.Editor;
using UnityEditor;

namespace Entitas.Unity.Editor
{
    public class DictionaryTypeDrawer : ITypeDrawer
    {
        static readonly Dictionary<Type, string> KeySearchTexts = new Dictionary<Type, string>();

        public bool HandlesType(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            var dictionary = (IDictionary)value;
            var keyType = memberType.GetGenericArguments()[0];
            var valueType = memberType.GetGenericArguments()[1];
            var targetType = target.GetType();
            KeySearchTexts.TryAdd(targetType, string.Empty);

            EditorGUILayout.BeginHorizontal();
            {
                if (dictionary.Count == 0)
                {
                    EditorGUILayout.LabelField(memberName, "empty");
                    KeySearchTexts[targetType] = string.Empty;
                }
                else
                {
                    EditorGUILayout.LabelField(memberName);
                }

                var keyTypeName = keyType.ToCompilableString().TypeName();
                var valueTypeName = valueType.ToCompilableString().TypeName();
                if (EditorLayout.MiniButton($"new <{keyTypeName}, {valueTypeName}>"))
                    if (EntityDrawer.CreateDefault(keyType, out var defaultKey))
                        if (EntityDrawer.CreateDefault(valueType, out var defaultValue))
                            dictionary[defaultKey] = defaultValue;
            }
            EditorGUILayout.EndHorizontal();

            if (dictionary.Count > 0)
            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indent + 1;

                if (dictionary.Count > 5)
                {
                    EditorGUILayout.Space();
                    KeySearchTexts[targetType] = EditorLayout.SearchTextField(KeySearchTexts[targetType]);
                }

                EditorGUILayout.Space();

                var keys = new ArrayList(dictionary.Keys);
                for (var i = 0; i < keys.Count; i++)
                {
                    var key = keys[i];
                    if (EditorLayout.MatchesSearchString(key.ToString().ToLower(), KeySearchTexts[targetType].ToLower()))
                    {
                        EntityDrawer.DrawObjectMember(keyType, "key", key,
                            target, (_, newValue) =>
                            {
                                var tmpValue = dictionary[key];
                                dictionary.Remove(key);
                                if (newValue != null)
                                {
                                    dictionary[newValue] = tmpValue;
                                }
                            });

                        EntityDrawer.DrawObjectMember(valueType, "value", dictionary[key],
                            target, (_, newValue) => dictionary[key] = newValue);

                        EditorGUILayout.Space();
                    }
                }

                EditorGUI.indentLevel = indent;
            }

            return dictionary;
        }
    }
}
