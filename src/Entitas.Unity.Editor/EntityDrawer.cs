using System;
using System.IO;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Reflection;
using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    public static partial class EntityDrawer
    {
        public static void DrawEntity(Entity entity)
        {
            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Destroy Entity"))
                entity.Destroy();

            GUI.backgroundColor = bgColor;

            DrawComponents(entity);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField($"Retained by ({entity.RetainCount})", EditorStyles.boldLabel);

            if (entity.Aerc is SafeAERC safeAerc)
            {
                EditorLayout.BeginVerticalBox();
                {
                    foreach (var owner in safeAerc.Owners.OrderBy(o => o.GetType().Name))
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(owner.ToString());
                            if (EditorLayout.MiniButton("Release"))
                                entity.Release(owner);

                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                EditorLayout.EndVerticalBox();
            }
        }

        public static void DrawMultipleEntities(Entity[] entities)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                var entity = entities[0];
                var index = DrawAddComponentMenu(entity);
                if (index >= 0)
                {
                    var componentType = entity.ContextInfo.ComponentTypes[index];
                    foreach (var e in entities)
                    {
                        var component = e.CreateComponent(index, componentType);
                        e.AddComponent(index, component);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;

            if (GUILayout.Button("Destroy selected entities"))
                foreach (var entity in entities)
                    entity.Destroy();

            GUI.backgroundColor = bgColor;

            EditorGUILayout.Space();

            foreach (var entity in entities)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(entity.ToString());

                    bgColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;

                    if (EditorLayout.MiniButton("Destroy Entity"))
                        entity.Destroy();

                    GUI.backgroundColor = bgColor;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        public static void DrawComponents(Entity entity)
        {
            var unfoldedComponents = GetUnfoldedComponents(entity);
            var componentMemberSearch = GetComponentMemberSearch(entity);

            EditorLayout.BeginVerticalBox();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField($"Components ({entity.GetComponents().Length})", EditorStyles.boldLabel);
                    if (EditorLayout.MiniButtonLeft("▸"))
                        for (var i = 0; i < unfoldedComponents.Length; i++)
                            unfoldedComponents[i] = false;

                    if (EditorLayout.MiniButtonRight("▾"))
                        for (var i = 0; i < unfoldedComponents.Length; i++)
                            unfoldedComponents[i] = true;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                var index = DrawAddComponentMenu(entity);
                if (index >= 0)
                {
                    var componentType = entity.ContextInfo.ComponentTypes[index];
                    var component = entity.CreateComponent(index, componentType);
                    entity.AddComponent(index, component);
                }

                EditorGUILayout.Space();

                ComponentNameSearchString = EditorLayout.SearchTextField(ComponentNameSearchString);

                EditorGUILayout.Space();

                var indexes = entity.GetComponentIndexes();
                var components = entity.GetComponents();
                for (var i = 0; i < components.Length; i++)
                    DrawComponent(unfoldedComponents, componentMemberSearch, entity, indexes[i], components[i]);
            }
            EditorLayout.EndVerticalBox();
        }

        public static void DrawComponent(bool[] unfoldedComponents, string[] componentMemberSearch, Entity entity, int index, IComponent component)
        {
            var componentType = component.GetType();
            var componentName = componentType.Name.RemoveSuffix("Component");
            if (EditorLayout.MatchesSearchString(componentName.ToLower(), ComponentNameSearchString.ToLower()))
            {
                EditorGUILayout.BeginVertical();
                {
                    if (!Attribute.IsDefined(componentType, typeof(DontDrawComponentAttribute)))
                    {
                        var memberInfos = componentType.GetPublicMemberInfos();
                        EditorGUILayout.BeginHorizontal();
                        {
                            if (memberInfos.Length == 0)
                            {
                                EditorGUILayout.LabelField(componentName, EditorStyles.boldLabel);
                            }
                            else
                            {
                                unfoldedComponents[index] = EditorLayout.Foldout(unfoldedComponents[index], componentName, FoldoutStyle);
                                if (unfoldedComponents[index])
                                {
                                    componentMemberSearch[index] = memberInfos.Length > 5
                                        ? EditorLayout.SearchTextField(componentMemberSearch[index])
                                        : string.Empty;
                                }
                            }

                            if (EditorLayout.MiniButton("-"))
                                entity.RemoveComponent(index);
                        }
                        EditorGUILayout.EndHorizontal();

                        if (unfoldedComponents[index])
                        {
                            var newComponent = entity.CreateComponent(index, componentType);
                            component.CopyPublicMemberValues(newComponent);

                            var changed = false;
                            var componentDrawer = GetComponentDrawer(componentType);
                            if (componentDrawer != null)
                            {
                                EditorGUI.BeginChangeCheck();
                                {
                                    componentDrawer.DrawComponent(newComponent);
                                }
                                changed = EditorGUI.EndChangeCheck();
                            }
                            else
                            {
                                foreach (var info in memberInfos)
                                {
                                    if (EditorLayout.MatchesSearchString(info.Name.ToLower(), componentMemberSearch[index].ToLower()))
                                    {
                                        var memberValue = info.GetValue(newComponent);
                                        var memberType = memberValue == null ? info.Type : memberValue.GetType();
                                        if (DrawObjectMember(memberType, info.Name, memberValue, newComponent, info.SetValue))
                                            changed = true;
                                    }
                                }
                            }

                            if (changed)
                                entity.ReplaceComponent(index, newComponent);
                            else
                                entity.GetComponentPool(index).Push(newComponent);
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField(componentName, "[DontDrawComponent]", EditorStyles.boldLabel);
                    }
                }
                EditorLayout.EndVerticalBox();
            }
        }

        public static bool DrawObjectMember(Type memberType, string memberName, object value, object target, Action<object, object> setValue)
        {
            if (value == null)
            {
                EditorGUI.BeginChangeCheck();
                {
                    var isUnityObject = memberType == typeof(UnityEngine.Object) || memberType.IsSubclassOf(typeof(UnityEngine.Object));
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (isUnityObject)
                            setValue(target, EditorGUILayout.ObjectField(memberName, (UnityEngine.Object)value, memberType, true));
                        else
                            EditorGUILayout.LabelField(memberName, "null");

                        if (EditorLayout.MiniButton($"new {memberType.ToCompilableString().TypeName()}"))
                        {
                            if (CreateDefault(memberType, out var defaultValue))
                                setValue(target, defaultValue);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                return EditorGUI.EndChangeCheck();
            }

            if (!memberType.IsValueType)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
            }

            EditorGUI.BeginChangeCheck();
            {
                var typeDrawer = GetTypeDrawer(memberType);
                if (typeDrawer != null)
                {
                    var newValue = typeDrawer.DrawAndGetNewValue(memberType, memberName, value, target);
                    setValue(target, newValue);
                }
                else
                {
                    var targetType = target.GetType();
                    var shouldDraw = !targetType.ImplementsInterface<IComponent>() || !Attribute.IsDefined(targetType, typeof(DontDrawComponentAttribute));
                    if (shouldDraw)
                    {
                        EditorGUILayout.LabelField(memberName, value.ToString());

                        var indent = EditorGUI.indentLevel;
                        EditorGUI.indentLevel += 1;

                        EditorGUILayout.BeginVertical();
                        {
                            foreach (var info in memberType.GetPublicMemberInfos())
                            {
                                var mValue = info.GetValue(value);
                                var mType = mValue == null ? info.Type : mValue.GetType();
                                DrawObjectMember(mType, info.Name, mValue, value, info.SetValue);
                                if (memberType.IsValueType)
                                    setValue(target, value);
                            }
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUI.indentLevel = indent;
                    }
                    else
                    {
                        DrawUnsupportedType(memberType, memberName, value);
                    }
                }

                if (!memberType.IsValueType)
                {
                    EditorGUILayout.EndVertical();
                    if (EditorLayout.MiniButton("×"))
                        setValue(target, null);

                    EditorGUILayout.EndHorizontal();
                }
            }

            return EditorGUI.EndChangeCheck();
        }

        public static bool CreateDefault(Type type, out object defaultValue)
        {
            try
            {
                defaultValue = Activator.CreateInstance(type);
                return true;
            }
            catch (Exception)
            {
                foreach (var creator in DefaultInstanceCreators)
                {
                    if (creator.HandlesType(type))
                    {
                        defaultValue = creator.CreateDefault(type);
                        return true;
                    }
                }
            }

            var typeName = type.ToCompilableString();
            if (EditorUtility.DisplayDialog(
                    "No IDefaultInstanceCreator found",
                    "There's no IDefaultInstanceCreator implementation to handle the type '" + typeName + "'.\n" +
                    "Providing an IDefaultInstanceCreator enables you to create instances for that type.\n\n" +
                    "Do you want to generate an IDefaultInstanceCreator implementation for '" + typeName + "'?\n",
                    "Generate",
                    "Cancel"
                ))
            {
                GenerateIDefaultInstanceCreator(typeName);
            }

            defaultValue = null;
            return false;
        }

        static int DrawAddComponentMenu(Entity entity)
        {
            var componentInfos = GetComponentInfos(entity)
                .Where(info => !entity.HasComponent(info.Index))
                .ToArray();
            var componentNames = componentInfos
                .Select(info => info.Name)
                .ToArray();
            var index = EditorGUILayout.Popup("Add Component", -1, componentNames);
            return index >= 0
                ? componentInfos[index].Index
                : -1;
        }

        static void DrawUnsupportedType(Type memberType, string memberName, object value)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(memberName, value.ToString());
                if (EditorLayout.MiniButton("Missing ITypeDrawer"))
                {
                    var typeName = memberType.ToCompilableString();
                    if (EditorUtility.DisplayDialog(
                            "No ITypeDrawer found",
                            "There's no ITypeDrawer implementation to handle the type '" + typeName + "'.\n" +
                            "Providing an ITypeDrawer enables you draw instances for that type.\n\n" +
                            "Do you want to generate an ITypeDrawer implementation for '" + typeName + "'?\n",
                            "Generate",
                            "Cancel"
                        ))
                    {
                        GenerateITypeDrawer(typeName);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void GenerateIDefaultInstanceCreator(string typeName)
        {
            var preferences = new Preferences("Entitas.properties", $"{Environment.UserName}.userproperties");
            var config = preferences.CreateAndConfigure<VisualDebuggingConfig>();
            var folder = config.defaultInstanceCreatorFolderPath;
            var filePath = folder + Path.DirectorySeparatorChar + "Default" + typeName.TypeName() + "InstanceCreator.cs";
            var template = DefaultInstanceCreatorTemplateFormat
                .Replace("${Type}", typeName)
                .Replace("${ShortType}", typeName.TypeName());
            GenerateTemplate(folder, filePath, template);
        }

        public static void GenerateITypeDrawer(string typeName)
        {
            var preferences = new Preferences("Entitas.properties", $"{Environment.UserName}.userproperties");
            var config = preferences.CreateAndConfigure<VisualDebuggingConfig>();
            var folder = config.typeDrawerFolderPath;
            var filePath = folder + Path.DirectorySeparatorChar + typeName.TypeName() + "TypeDrawer.cs";
            var template = TypeDrawerTemplateFormat
                .Replace("${Type}", typeName)
                .Replace("${ShortType}", typeName.TypeName());
            GenerateTemplate(folder, filePath, template);
        }

        static void GenerateTemplate(string folder, string filePath, string template)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            File.WriteAllText(filePath, template);
            EditorApplication.isPlaying = false;
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(filePath);
        }

        const string DefaultInstanceCreatorTemplateFormat =
            @"using System;
using Entitas.VisualDebugging.Unity.Editor;

public class Default${ShortType}InstanceCreator : IDefaultInstanceCreator {

    public bool HandlesType(Type type) {
        return type == typeof(${Type});
    }

    public object CreateDefault(Type type) {
        // TODO return an instance of type ${Type}
        throw new NotImplementedException();
    }
}
";

        const string TypeDrawerTemplateFormat =
            @"using System;
using Entitas;
using Entitas.VisualDebugging.Unity.Editor;

public class ${ShortType}TypeDrawer : ITypeDrawer {

    public bool HandlesType(Type type) {
        return type == typeof(${Type});
    }

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
        // TODO draw the type ${Type}
        throw new NotImplementedException();
    }
}
";
    }
}
