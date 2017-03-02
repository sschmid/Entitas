using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    public static partial class EntityDrawer {

        public static void DrawEntity(IContext context, IEntity entity) {
            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if(GUILayout.Button("Destroy Entity")) {
                context.DestroyEntity(entity);
            }
            GUI.backgroundColor = bgColor;

            DrawComponents(context, entity);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Retained by (" + entity.retainCount + ")", EditorStyles.boldLabel);

            #if !ENTITAS_FAST_AND_UNSAFE

            EntitasEditorLayout.BeginVerticalBox();
            {
                foreach(var owner in entity.owners.OrderBy(o => o.GetType().Name)) {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(owner.ToString());
                        if(EntitasEditorLayout.MiniButton("Release")) {
                            entity.Release(owner);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EntitasEditorLayout.EndVerticalBox();

            #endif
        }

        public static void DrawMultipleEntities(IContext context, IEntity[] entities) {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                var entity = entities[0];
                var index = drawAddComponentMenu(context);
                if(index >= 0) {
                    var componentType = entity.contextInfo.componentTypes[index];
                    foreach(var e in entities) {
                        var component = e.CreateComponent(index, componentType);
                        e.AddComponent(index, component);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;

            if(GUILayout.Button("Destroy selected entities")) {
                foreach(var e in entities) {
                    context.DestroyEntity(e);
                }
            }

            GUI.backgroundColor = bgColor;

            EditorGUILayout.Space();

            foreach(var e in entities) {

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(e.ToString());

                    bgColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;

                    if(EntitasEditorLayout.MiniButton("Destroy Entity")) {
                        context.DestroyEntity(e);
                    }

                    GUI.backgroundColor = bgColor;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        public static void DrawComponents(IContext context, IEntity entity) {
            var unfoldedComponents = getUnfoldedComponents(context);
            var componentMemberSearch = getComponentMemberSearch(context);

            EntitasEditorLayout.BeginVerticalBox();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Components (" + entity.GetComponents().Length + ")", EditorStyles.boldLabel);
                    if(EntitasEditorLayout.MiniButtonLeft("▸")) {
                        for (int i = 0; i < unfoldedComponents.Length; i++) {
                            unfoldedComponents[i] = false;
                        }
                    }
                    if(EntitasEditorLayout.MiniButtonRight("▾")) {
                        for (int i = 0; i < unfoldedComponents.Length; i++) {
                            unfoldedComponents[i] = true;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                var index = drawAddComponentMenu(context);
                if(index >= 0) {
                    var componentType = entity.contextInfo.componentTypes[index];
                    var component = entity.CreateComponent(index, componentType);
                    entity.AddComponent(index, component);
                }

                EditorGUILayout.Space();

                componentNameSearchString = EntitasEditorLayout.SearchTextField(componentNameSearchString);

                EditorGUILayout.Space();

                var indices = entity.GetComponentIndices();
                var components = entity.GetComponents();
                for (int i = 0; i < components.Length; i++) {
                    DrawComponent(unfoldedComponents, componentMemberSearch, context, entity, indices[i], components[i]);
                }
            }
            EntitasEditorLayout.EndVerticalBox();
        }

        public static void DrawComponent(bool[] unfoldedComponents, string[] componentMemberSearch, IContext context, IEntity entity, int index, IComponent component) {
            var componentType = component.GetType();
            var componentName = componentType.Name.RemoveComponentSuffix();
            if(EntitasEditorLayout.MatchesSearchString(componentName.ToLower(), componentNameSearchString.ToLower())) {
                var boxStyle = getColoredBoxStyle(context, index);
                EditorGUILayout.BeginVertical(boxStyle);
                {
                    var memberInfos = componentType.GetPublicMemberInfos();
                    EditorGUILayout.BeginHorizontal();
                    {
                        if(memberInfos.Count == 0) {
                            EditorGUILayout.LabelField(componentName, EditorStyles.boldLabel);
                        } else {
                            unfoldedComponents[index] = EntitasEditorLayout.Foldout(unfoldedComponents[index], componentName, foldoutStyle);
                            componentMemberSearch[index] = memberInfos.Count > 5
                                                                      ? EntitasEditorLayout.SearchTextField(componentMemberSearch[index])
                                                                      : string.Empty;
                        }
                        if(EntitasEditorLayout.MiniButton("-")) {
                            entity.RemoveComponent(index);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if(unfoldedComponents[index]) {
                        var newComponent = entity.CreateComponent(index, componentType);
                        component.CopyPublicMemberValues(newComponent);

                        var changed = false;
                        var componentDrawer = getComponentDrawer(componentType);
                        if(componentDrawer != null) {
                            EditorGUI.BeginChangeCheck();
                            {
                                componentDrawer.DrawComponent(newComponent);
                            }
                            changed = EditorGUI.EndChangeCheck();
                        } else {
                            foreach(var info in memberInfos) {
                                if(EntitasEditorLayout.MatchesSearchString(info.name.ToLower(), componentMemberSearch[index].ToLower())) {
                                    if(DrawComponentMember(info.type, info.name, info.GetValue(newComponent), newComponent, info.SetValue)) {
                                        changed = true;
                                    }
                                }
                            }
                        }

                        if(changed) {
                            entity.ReplaceComponent(index, newComponent);
                        } else {
                            entity.GetComponentPool(index).Push(newComponent);
                        }
                    }
                }
                EntitasEditorLayout.EndVerticalBox();
            }
        }

        public static bool DrawComponentMember(Type memberType, string memberName, object value, IComponent component, Action<IComponent, object> setValue) {
            if(value == null) {
                EditorGUI.BeginChangeCheck();
                {
                    var isUnityObject = memberType == typeof(UnityEngine.Object) || memberType.IsSubclassOf(typeof(UnityEngine.Object));
                    EditorGUILayout.BeginHorizontal();
                    {
                        if(isUnityObject) {
                            setValue(component, EditorGUILayout.ObjectField(memberName, (UnityEngine.Object)value, memberType, true));
                        } else {
                            EditorGUILayout.LabelField(memberName, "null");
                        }

                        if(EntitasEditorLayout.MiniButton("new " + memberType.ToCompilableString().ShortTypeName())) {
                            object defaultValue;
                            if(CreateDefault(memberType, out defaultValue)) {
                                setValue(component, defaultValue);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                return EditorGUI.EndChangeCheck();
            }

            EditorGUI.BeginChangeCheck();
            {
                if(!memberType.IsValueType) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                }

                var typeDrawer = getTypeDrawer(memberType);
                if(typeDrawer != null) {
                    setValue(component, typeDrawer.DrawAndGetNewValue(memberType, memberName, value, component));
                } else {
                    drawUnsupportedType(memberType, memberName, value);
                }

                if(!memberType.IsValueType) {
                    EditorGUILayout.EndVertical();
                    if(EntitasEditorLayout.MiniButton("×")) {
                        setValue(component, null);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            return EditorGUI.EndChangeCheck();
        }

        public static bool CreateDefault(Type type, out object defaultValue) {
            try {
                defaultValue = Activator.CreateInstance(type);
                return true;
            } catch (Exception) {
                foreach(var creator in _defaultInstanceCreators) {
                    if(creator.HandlesType(type)) {
                        defaultValue = creator.CreateDefault(type);
                        return true;
                    }
                }
            }

            var typeName = type.ToCompilableString();
            if(EditorUtility.DisplayDialog(
                    "No IDefaultInstanceCreator found",
                    "There's no IDefaultInstanceCreator implementation to handle the type '" + typeName + "'.\n" +
                    "Providing an IDefaultInstanceCreator enables you to create instances for that type.\n\n" +
                    "Do you want to generate an IDefaultInstanceCreator implementation for '" + typeName + "'?\n",
                    "Generate",
                    "Cancel"
                )) {
                generateIDefaultInstanceCreator(typeName);
            }

            defaultValue = null;
            return false;
        }

        static int drawAddComponentMenu(IContext context) {
            var componentInfos = getComponentInfos(context);
            var componentNames = componentInfos.Select(info => info.name).ToArray();
            var index = EditorGUILayout.Popup("Add Component", -1, componentNames);
            if(index >= 0) {
                return componentInfos[index].index;
            }

            return -1;
        }

        static void drawUnsupportedType(Type memberType, string memberName, object value) {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(memberName, value.ToString());
                if(EntitasEditorLayout.MiniButton("Missing ITypeDrawer")) {
                    var typeName = memberType.ToCompilableString();
                    if(EditorUtility.DisplayDialog(
                            "No ITypeDrawer found",
                            "There's no ITypeDrawer implementation to handle the type '" + typeName + "'.\n" +
                            "Providing an ITypeDrawer enables you draw instances for that type.\n\n" +
                            "Do you want to generate an ITypeDrawer implementation for '" + typeName + "'?\n",
                            "Generate",
                            "Cancel"
                        )) {
                        generateITypeDrawer(typeName);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        static void generateIDefaultInstanceCreator(string typeName) {
            var config = new VisualDebuggingConfig(EntitasPreferences.LoadConfig());
            var folder = config.defaultInstanceCreatorFolderPath;
            var filePath = folder + "Default_type_InstanceCreator.cs";
            var template = string.Format(DEFAULT_INSTANCE_CREATOR_TEMPLATE_FORMAT, typeName);
            generateTemplate(folder, filePath, template);
        }

        static void generateITypeDrawer(string typeName) {
            var config = new VisualDebuggingConfig(EntitasPreferences.LoadConfig());
            var folder = config.typeDrawerFolderPath;
            var filePath = folder + "Type_TypeDrawer.cs";
            var template = string.Format(TYPE_DRAWER_TEMPLATE_FORMAT, typeName);
            generateTemplate(folder, filePath, template);
        }

        static void generateTemplate(string folder, string filePath, string template) {
            if(!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            File.WriteAllText(filePath, template);
            AssetDatabase.Refresh();
            EditorApplication.isPlaying = false;
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(filePath);
        }

        const string DEFAULT_INSTANCE_CREATOR_TEMPLATE_FORMAT = @"using System;
using Entitas.Unity.VisualDebugging;

// Please rename the class and the file
public class Default_type_InstanceCreator : IDefaultInstanceCreator {{

    public bool HandlesType(Type type) {{
        return type == typeof({0});
    }}

    public object CreateDefault(Type type) {{
        // return an instance of type {0}
        throw new NotImplementedException();
    }}
}}
";

        const string TYPE_DRAWER_TEMPLATE_FORMAT = @"using System;
using Entitas;
using Entitas.Unity.VisualDebugging;

// Please rename the class and the file
public class Type_TypeDrawer : ITypeDrawer {{

    public bool HandlesType(Type type) {{
        return type == typeof({0});
    }}

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, IEntity entity, int index, IComponent component) {{
        // draw the type {0}
        throw new NotImplementedException();
    }}
}}";
    }
}
