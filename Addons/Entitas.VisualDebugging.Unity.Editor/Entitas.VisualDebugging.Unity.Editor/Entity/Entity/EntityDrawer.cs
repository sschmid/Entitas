using System;
using System.IO;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using DesperateDevs.Utils;
using UnityEditor;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity.Editor
{
    public static partial class EntityDrawer
    {
        public static void DrawEntity(IEntity entity)
        {
            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Destroy Entity"))
            {
                entity.Destroy();
            }

            GUI.backgroundColor = bgColor;

            DrawComponents(entity);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Retained by (" + entity.retainCount + ")", EditorStyles.boldLabel);

            var safeAerc = entity.aerc as SafeAERC;
            if (safeAerc != null)
            {
                EditorLayout.BeginVerticalBox();
                {
                    foreach (var owner in safeAerc.owners.OrderBy(o => o.GetType().Name))
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(owner.ToString());
                            if (EditorLayout.MiniButton("Release"))
                            {
                                entity.Release(owner);
                            }

                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                EditorLayout.EndVerticalBox();
            }
        }

        public static void DrawMultipleEntities(IEntity[] entities)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                var entity = entities[0];
                var index = drawAddComponentMenu(entity);
                if (index >= 0)
                {
                    var componentType = entity.contextInfo.componentTypes[index];
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
            {
                foreach (var e in entities)
                {
                    e.Destroy();
                }
            }

            GUI.backgroundColor = bgColor;

            EditorGUILayout.Space();

            foreach (var e in entities)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(e.ToString());

                    bgColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;

                    if (EditorLayout.MiniButton("Destroy Entity"))
                    {
                        e.Destroy();
                    }

                    GUI.backgroundColor = bgColor;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        public static void DrawComponents(IEntity entity)
        {
            var unfoldedComponents = getUnfoldedComponents(entity);
            var componentMemberSearch = getComponentMemberSearch(entity);

            EditorLayout.BeginVerticalBox();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Components (" + entity.GetComponents().Length + ")", EditorStyles.boldLabel);
                    if (EditorLayout.MiniButtonLeft("▸"))
                    {
                        for (int i = 0; i < unfoldedComponents.Length; i++)
                        {
                            unfoldedComponents[i] = false;
                        }
                    }

                    if (EditorLayout.MiniButtonRight("▾"))
                    {
                        for (int i = 0; i < unfoldedComponents.Length; i++)
                        {
                            unfoldedComponents[i] = true;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                var index = drawAddComponentMenu(entity);
                if (index >= 0)
                {
                    var componentType = entity.contextInfo.componentTypes[index];
                    var component = entity.CreateComponent(index, componentType);
                    entity.AddComponent(index, component);
                }

                EditorGUILayout.Space();

                componentNameSearchString = EditorLayout.SearchTextField(componentNameSearchString);

                EditorGUILayout.Space();

                var indices = entity.GetComponentIndices();
                var components = entity.GetComponents();
                for (int i = 0; i < components.Length; i++)
                {
                    DrawComponent(unfoldedComponents, componentMemberSearch, entity, indices[i], components[i]);
                }
            }
            EditorLayout.EndVerticalBox();
        }

        public static void DrawComponent(bool[] unfoldedComponents, string[] componentMemberSearch, IEntity entity, int index, IComponent component)
        {
            var componentType = component.GetType();
            var componentName = componentType.Name.RemoveComponentSuffix();
            if (EditorLayout.MatchesSearchString(componentName.ToLower(), componentNameSearchString.ToLower()))
            {
                var boxStyle = getColoredBoxStyle(entity, index);
                EditorGUILayout.BeginVertical(boxStyle);
                {
                    if (!Attribute.IsDefined(componentType, typeof(DontDrawComponentAttribute)))
                    {
                        var memberInfos = componentType.GetPublicMemberInfos();
                        EditorGUILayout.BeginHorizontal();
                        {
                            if (memberInfos.Count == 0)
                            {
                                EditorGUILayout.LabelField(componentName, EditorStyles.boldLabel);
                            }
                            else
                            {
                                unfoldedComponents[index] = EditorLayout.Foldout(unfoldedComponents[index], componentName, foldoutStyle);
                                if (unfoldedComponents[index])
                                {
                                    componentMemberSearch[index] = memberInfos.Count > 5
                                        ? EditorLayout.SearchTextField(componentMemberSearch[index])
                                        : string.Empty;
                                }
                            }

                            if (EditorLayout.MiniButton("-"))
                            {
                                entity.RemoveComponent(index);
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        if (unfoldedComponents[index])
                        {
                            var newComponent = entity.CreateComponent(index, componentType);
                            component.CopyPublicMemberValues(newComponent);

                            var changed = false;
                            var componentDrawer = getComponentDrawer(componentType);
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
                                    if (EditorLayout.MatchesSearchString(info.name.ToLower(), componentMemberSearch[index].ToLower()))
                                    {
                                        var memberValue = info.GetValue(newComponent);
                                        var memberType = memberValue == null ? info.type : memberValue.GetType();
                                        if (DrawObjectMember(memberType, info.name, memberValue, newComponent, info.SetValue))
                                        {
                                            changed = true;
                                        }
                                    }
                                }
                            }

                            if (changed)
                            {
                                entity.ReplaceComponent(index, newComponent);
                            }
                            else
                            {
                                entity.GetComponentPool(index).Push(newComponent);
                            }
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
                        {
                            setValue(target, EditorGUILayout.ObjectField(memberName, (UnityEngine.Object)value, memberType, true));
                        }
                        else
                        {
                            EditorGUILayout.LabelField(memberName, "null");
                        }

                        if (EditorLayout.MiniButton("new " + memberType.ToCompilableString().ShortTypeName()))
                        {
                            object defaultValue;
                            if (CreateDefault(memberType, out defaultValue))
                            {
                                setValue(target, defaultValue);
                            }
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
                var typeDrawer = getTypeDrawer(memberType);
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
                                var mType = mValue == null ? info.type : mValue.GetType();
                                DrawObjectMember(mType, info.name, mValue, value, info.SetValue);
                                if (memberType.IsValueType)
                                {
                                    setValue(target, value);
                                }
                            }
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUI.indentLevel = indent;
                    }
                    else
                    {
                        drawUnsupportedType(memberType, memberName, value);
                    }
                }

                if (!memberType.IsValueType)
                {
                    EditorGUILayout.EndVertical();
                    if (EditorLayout.MiniButton("×"))
                    {
                        setValue(target, null);
                    }

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
                foreach (var creator in _defaultInstanceCreators)
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

        static int drawAddComponentMenu(IEntity entity)
        {
            var componentInfos = getComponentInfos(entity)
                .Where(info => !entity.HasComponent(info.index))
                .ToArray();
            var componentNames = componentInfos
                .Select(info => info.name)
                .ToArray();
            var index = EditorGUILayout.Popup("Add Component", -1, componentNames);
            if (index >= 0)
            {
                return componentInfos[index].index;
            }

            return -1;
        }

        static void drawUnsupportedType(Type memberType, string memberName, object value)
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
            var preferences = new Preferences("Entitas.properties", Environment.UserName + ".userproperties");
            var config = preferences.CreateAndConfigure<VisualDebuggingConfig>();
            var folder = config.defaultInstanceCreatorFolderPath;
            var filePath = folder + Path.DirectorySeparatorChar + "Default" + typeName.ShortTypeName() + "InstanceCreator.cs";
            var template = DEFAULT_INSTANCE_CREATOR_TEMPLATE_FORMAT
                .Replace("${Type}", typeName)
                .Replace("${ShortType}", typeName.ShortTypeName());
            generateTemplate(folder, filePath, template);
        }

        public static void GenerateITypeDrawer(string typeName)
        {
            var preferences = new Preferences("Entitas.properties", Environment.UserName + ".userproperties");
            var config = preferences.CreateAndConfigure<VisualDebuggingConfig>();
            var folder = config.typeDrawerFolderPath;
            var filePath = folder + Path.DirectorySeparatorChar + typeName.ShortTypeName() + "TypeDrawer.cs";
            var template = TYPE_DRAWER_TEMPLATE_FORMAT
                .Replace("${Type}", typeName)
                .Replace("${ShortType}", typeName.ShortTypeName());
            generateTemplate(folder, filePath, template);
        }

        static void generateTemplate(string folder, string filePath, string template)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllText(filePath, template);
            EditorApplication.isPlaying = false;
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(filePath);
        }

        const string DEFAULT_INSTANCE_CREATOR_TEMPLATE_FORMAT =
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

        const string TYPE_DRAWER_TEMPLATE_FORMAT =
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
