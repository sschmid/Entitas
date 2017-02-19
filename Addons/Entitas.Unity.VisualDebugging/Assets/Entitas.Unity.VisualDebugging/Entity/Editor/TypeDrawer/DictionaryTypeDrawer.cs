using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {

    public class DictionaryTypeDrawer : ITypeDrawer {

        public bool HandlesType(Type type) {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, IEntity entity, int index, IComponent component) {
            var dictionary = (IDictionary)value;
            var keyType = memberType.GetGenericArguments()[0];
            var valueType = memberType.GetGenericArguments()[1];

            EditorGUILayout.BeginHorizontal();
            {
                if(dictionary.Count == 0) {
                    EditorGUILayout.LabelField(memberName, "empty");
                } else {
                    EditorGUILayout.LabelField(memberName);
                }
                var keyTypeName = keyType.ToCompilableString().ShortTypeName();
                var valueTypeName = valueType.ToCompilableString().ShortTypeName();
                if(EntitasEditorLayout.MiniButton("new <" + keyTypeName + ", " + valueTypeName + ">")) {
                    object defaultKey;
                    if(EntityDrawer.CreateDefault(keyType, out defaultKey)) {
                        object defaultValue;
                        if(EntityDrawer.CreateDefault(valueType, out defaultValue)) {
                            dictionary[defaultKey] = defaultValue;
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if(dictionary.Count > 0) {
                EditorGUILayout.Space();
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indent + 1;

                var keys = new ArrayList(dictionary.Keys);
                for (int i = 0; i < keys.Count; i++) {
                    var key = keys[i];
                    EntityDrawer.DrawAndSetElement(keyType, "key", key,
                        entity, index, component, (newComponent, newValue) => {
                        var tmpValue = dictionary[key];
                        dictionary.Remove(key);
                        if(newValue != null) {
                            dictionary[newValue] = tmpValue;
                        }
                    });

                    EntityDrawer.DrawAndSetElement(valueType, "value", dictionary[key],
                        entity, index, component, (newComponent, newValue) => dictionary[key] = newValue);

                    EditorGUILayout.Space();
                }

                EditorGUI.indentLevel = indent;
            }

            return dictionary;
        }
    }
}
