using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Entitas.CodeGenerator.Api;

namespace Entitas.Unity.VisualDebugging {

    public static class EntityDrawer {

        struct ComponentInfo {
            public int index;
            public string name;
            public Type type;
        }

        static Dictionary<IContext, bool[]> _contextToUnfoldedComponents;
        static Dictionary<IContext, string[]> _contextToComponentMemberSearch;
        static GUIStyle _foldoutStyle;
        static Dictionary<int, GUIStyle[]> _coloredBoxStyles;

        static IDefaultInstanceCreator[] _defaultInstanceCreators;
        static ITypeDrawer[] _typeDrawers;
        static IComponentDrawer[] _componentDrawers;
        static ITypeEqualityComparer[] _typeEqualityComparers;

        static string _componentNameSearchString = string.Empty;

        static bool _isInitialized;

        public static void Initialize() {
            if(!_isInitialized) {
                _isInitialized = true;

                _contextToUnfoldedComponents = new Dictionary<IContext, bool[]>();
                _contextToComponentMemberSearch = new Dictionary<IContext, string[]>();

                var types = Assembly.GetAssembly(typeof(EntityInspector)).GetTypes();
                _defaultInstanceCreators = types
                    .Where(type => type.ImplementsInterface<IDefaultInstanceCreator>())
                    .Select(type => (IDefaultInstanceCreator)Activator.CreateInstance(type))
                    .ToArray();

                _typeDrawers = types
                    .Where(type => type.ImplementsInterface<ITypeDrawer>())
                    .Select(type => (ITypeDrawer)Activator.CreateInstance(type))
                    .ToArray();

                _componentDrawers = types
                    .Where(type => type.ImplementsInterface<IComponentDrawer>())
                    .Select(type => (IComponentDrawer)Activator.CreateInstance(type))
                    .ToArray();

                _typeEqualityComparers = types
                    .Where(type => type.ImplementsInterface<ITypeEqualityComparer>())
                    .Select(type => (ITypeEqualityComparer)Activator.CreateInstance(type))
                    .ToArray();
            }

            // Unity bug
            // NullReferenceException at EditorStyles.cs:136 when entering play-mode
            try {
                _foldoutStyle = new GUIStyle(EditorStyles.foldout);
                _foldoutStyle.fontStyle = FontStyle.Bold;
            } catch (Exception) {
            }

            _coloredBoxStyles = new Dictionary<int, GUIStyle[]>();
        }

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

        public static void DrawComponents(IContext context, IEntity entity) {
            bool[] unfoldedComponents;
            if(!_contextToUnfoldedComponents.TryGetValue(context, out unfoldedComponents)) {
                unfoldedComponents = new bool[context.totalComponents];
                for (int i = 0; i < unfoldedComponents.Length; i++) {
                    unfoldedComponents[i] = true;
                }
                _contextToUnfoldedComponents.Add(context, unfoldedComponents);
            }

            string[] componentMemberSearch;
            if(!_contextToComponentMemberSearch.TryGetValue(context, out componentMemberSearch)) {
                componentMemberSearch = new string[context.totalComponents];
                for (int i = 0; i < componentMemberSearch.Length; i++) {
                    componentMemberSearch[i] = string.Empty;
                }
                _contextToComponentMemberSearch.Add(context, componentMemberSearch);
            }

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

                var index = drawAddComponentMenu(entity);
                if(index >= 0) {
                    var componentType = entity.contextInfo.componentTypes[index];
                    var component = (IComponent)Activator.CreateInstance(componentType);
                    entity.AddComponent(index, component);
                }

                EditorGUILayout.Space();

                _componentNameSearchString = EntitasEditorLayout.SearchTextField(_componentNameSearchString);

                EditorGUILayout.Space();

                var indices = entity.GetComponentIndices();
                var components = entity.GetComponents();
                for (int i = 0; i < components.Length; i++) {
                    DrawComponent(unfoldedComponents, componentMemberSearch, entity, indices[i], components[i]);
                }
            }
            EntitasEditorLayout.EndVerticalBox();
        }

        public static void DrawMultipleEntities(IContext context, IEntity[] entities) {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                var entity = entities[0];
                var index = drawAddComponentMenu(entity);
                if(index >= 0) {
                    var componentType = entity.contextInfo.componentTypes[index];
                    foreach(var e in entities) {
                        var component = (IComponent)Activator.CreateInstance(componentType);
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

        public static void DrawComponent(bool[] unfoldedComponents, string[] componentMemberSearch, IEntity entity, int index, IComponent component) {
            var componentType = component.GetType();
            var componentName = componentType.Name.RemoveComponentSuffix();
            if(EntitasEditorLayout.MatchesSearchString(componentName.ToLower(), _componentNameSearchString.ToLower())) {
                var boxStyle = getColoredBoxStyle(entity.totalComponents, index);
                EditorGUILayout.BeginVertical(boxStyle);
                {
                    var memberInfos = componentType.GetPublicMemberInfos();
                    EditorGUILayout.BeginHorizontal();
                    {
                        if(memberInfos.Count == 0) {
                            EditorGUILayout.LabelField(componentName, EditorStyles.boldLabel);
                        } else {
                            unfoldedComponents[index] = EntitasEditorLayout.Foldout(unfoldedComponents[index], componentName, _foldoutStyle);
                            if(memberInfos.Count > 5) {
                                componentMemberSearch[index] = EntitasEditorLayout.SearchTextField(componentMemberSearch[index]);
                            } else {
                                componentMemberSearch[index] = string.Empty;
                            }
                        }
                        if(EntitasEditorLayout.MiniButton("-")) {
                            entity.RemoveComponent(index);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if(unfoldedComponents[index]) {
                        var componentDrawer = getComponentDrawer(componentType);
                        if(componentDrawer != null) {
                            var newComponent = entity.CreateComponent(index, componentType);
                            component.CopyPublicMemberValues(newComponent);
                            EditorGUI.BeginChangeCheck();
                            {
                                componentDrawer.DrawComponent(newComponent);
                            }
                            var changed = EditorGUI.EndChangeCheck();
                            if(changed) {
                                entity.ReplaceComponent(index, newComponent);
                            } else {
                                entity.GetComponentPool(index).Push(newComponent);
                            }
                        } else {
                            foreach(var info in memberInfos) {
                                if(EntitasEditorLayout.MatchesSearchString(info.name.ToLower(), componentMemberSearch[index].ToLower())) {
                                    DrawAndSetElement(info.type, info.name, info.GetValue(component),
                                        entity, index, component, info.SetValue);
                                }
                            }
                        }
                    }
                }
                EntitasEditorLayout.EndVerticalBox();
            }
        }

        public static void DrawAndSetElement(Type memberType, string memberName, object value, IEntity entity, int index, IComponent component, Action<IComponent, object> setValue) {
            var newValue = DrawAndGetNewValue(memberType, memberName, value, entity, index, component);
            if(DidValueChange(value, newValue)) {
                var newComponent = entity.CreateComponent(index, component.GetType());
                component.CopyPublicMemberValues(newComponent);
                setValue(newComponent, newValue);
                entity.ReplaceComponent(index, newComponent);
            }
        }

        public static bool DidValueChange(object value, object newValue) {
            if((value == null && newValue != null) || (value != null && newValue == null)) {
                return true;
            }

            if(value != null && newValue != null) {
                var comparer = getTypeEqualityComparer(value.GetType());
                if(comparer != null && !comparer.Equals(value, newValue)) {
                    return true;
                }

                if(!newValue.Equals(value)) {
                    return true;
                }
            }

            return false;
        }

        public static object DrawAndGetNewValue(Type memberType, string memberName, object value, IEntity entity, int index, IComponent component) {
            if(value == null) {
                var isUnityObject = memberType == typeof(UnityEngine.Object) || memberType.IsSubclassOf(typeof(UnityEngine.Object));
                EditorGUILayout.BeginHorizontal();
                {
                    if(isUnityObject) {
                        value = EditorGUILayout.ObjectField(memberName, (UnityEngine.Object)value, memberType, true);
                    } else {
                        EditorGUILayout.LabelField(memberName, "null");
                    }

                    if(EntitasEditorLayout.MiniButton("new " + memberType.ToCompilableString().ShortTypeName())) {
                        object defaultValue;
                        if(CreateDefault(memberType, out defaultValue)) {
                            value = defaultValue;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                return value;
            }

            if(!memberType.IsValueType) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
            }

            var typeDrawer = getTypeDrawer(memberType);
            if(typeDrawer != null) {
                value = typeDrawer.DrawAndGetNewValue(memberType, memberName, value, entity, index, component);
            } else {
                drawUnsupportedType(memberType, memberName, value);
            }

            if(!memberType.IsValueType) {
                EditorGUILayout.EndVertical();
                if(EntitasEditorLayout.MiniButton("×")) {
                    value = null;
                }
                EditorGUILayout.EndHorizontal();
            }

            return value;
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

        static int drawAddComponentMenu(IEntity entity) {
            var componentInfos = getComponentInfos(entity);
            var componentNames = componentInfos.Select(info => info.name).ToArray();
            var index = EditorGUILayout.Popup("Add Component", -1, componentNames);
            if(index >= 0) {
                return componentInfos[index].index;
            }

            return -1;
        }

        static ComponentInfo[] getComponentInfos(IEntity entity) {
            var infos = new List<ComponentInfo>(entity.contextInfo.componentTypes.Length);
            for (int i = 0; i < entity.contextInfo.componentTypes.Length; ++i) {
                var type = entity.contextInfo.componentTypes[i];
                var name = entity.contextInfo.componentNames[i];
                infos.Add(new ComponentInfo {
                    index = i,
                    name = name,
                    type = type
                });
            }

            return infos.ToArray();
        }

        static GUIStyle getColoredBoxStyle(int totalComponents, int index) {
            GUIStyle[] styles;
            if(!_coloredBoxStyles.TryGetValue(totalComponents, out styles)) {
                styles = new GUIStyle[totalComponents];
                for (int i = 0; i < styles.Length; i++) {
                    var hue = (float)i / (float)totalComponents;
                    var componentColor = Color.HSVToRGB(hue, 0.7f, 1f);
                    componentColor.a = 0.15f;
                    var style = new GUIStyle(GUI.skin.box);
                    style.normal.background = createTexture(2, 2, componentColor);
                    styles[i] = style;
                }
                _coloredBoxStyles.Add(totalComponents, styles);
            }

            return styles[index];
        }

        static Texture2D createTexture(int width, int height, Color color) {
            var pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; ++i) {
                pixels[i] = color;
            }
            var result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }

        static ITypeDrawer getTypeDrawer(Type type) {
            foreach(var drawer in _typeDrawers) {
                if(drawer.HandlesType(type)) {
                    return drawer;
                }
            }

            return null;
        }

        static IComponentDrawer getComponentDrawer(Type type) {
            foreach(var drawer in _componentDrawers) {
                if(drawer.HandlesType(type)) {
                    return drawer;
                }
            }

            return null;
        }

        static ITypeEqualityComparer getTypeEqualityComparer(Type type) {
            foreach(var comparer in _typeEqualityComparers) {
                if(comparer.HandlesType(type)) {
                    return comparer;
                }
            }

            return null;
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
