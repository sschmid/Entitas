using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Entitas;
using Entitas.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    [CustomEditor(typeof(EntityDebugBehaviour)), CanEditMultipleObjects]
    public class EntityDebugEditor : Editor {
        GUIStyle _foldoutStyle;
        static IDefaultInstanceCreator[] _defaultInstanceCreators;
        static ITypeDrawer[] _typeDrawers;

        void Awake() {
            _foldoutStyle = new GUIStyle(EditorStyles.foldout);
            _foldoutStyle.fontStyle = FontStyle.Bold;

            var types = Assembly.GetAssembly(typeof(EntityDebugEditor)).GetTypes();

            _defaultInstanceCreators = types
                .Where(type => type.GetInterfaces().Contains(typeof(IDefaultInstanceCreator)))
                .Select(type => (IDefaultInstanceCreator)Activator.CreateInstance(type))
                .ToArray();

            _typeDrawers = types
                .Where(type => type.GetInterfaces().Contains(typeof(ITypeDrawer)))
                .Select(type => (ITypeDrawer)Activator.CreateInstance(type))
                .ToArray();
        }

        public override void OnInspectorGUI() {
            if (targets.Length == 1) {
                drawSingleTarget();
            } else {
                drawMultiTargets();
            }
            EditorUtility.SetDirty(target);
        }

        void drawSingleTarget() {
            var debugBehaviour = (EntityDebugBehaviour)target;
            var pool = debugBehaviour.pool;
            var entity = debugBehaviour.entity;

            if (GUILayout.Button("Destroy Entity")) {
                pool.DestroyEntity(entity);
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Components (" + entity.GetComponents().Length + ")", EditorStyles.boldLabel);
            if (GUILayout.Button("▸", GUILayout.Width(21), GUILayout.Height(14))) {
                debugBehaviour.FoldAllComponents();
            }
            if (GUILayout.Button("▾", GUILayout.Width(21), GUILayout.Height(14))) {
                debugBehaviour.UnfoldAllComponents();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            var indices = entity.GetComponentIndices();
            var components = entity.GetComponents();
            for (int i = 0; i < components.Length; i++) {
                drawComponent(debugBehaviour, entity, indices[i], components[i]);
            }
            EditorGUILayout.EndVertical();
        }

        void drawMultiTargets() {
            if (GUILayout.Button("Destroy selected entities")) {
                foreach (var t in targets) {
                    var debugBehaviour = (EntityDebugBehaviour)t;
                    var pool = debugBehaviour.pool;
                    var entity = debugBehaviour.entity;
                    pool.DestroyEntity(entity);
                }
            }

            EditorGUILayout.Space();

            foreach (var t in targets) {
                var debugBehaviour = (EntityDebugBehaviour)t;
                var pool = debugBehaviour.pool;
                var entity = debugBehaviour.entity;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(entity.ToString());
                if (GUILayout.Button("Destroy Entity")) {
                    pool.DestroyEntity(entity);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        void drawComponent(EntityDebugBehaviour debugBehaviour, Entity entity, int index, IComponent component) {
            var componentType = component.GetType();
            var fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            if (fields.Length == 0) {
                EditorGUILayout.LabelField(componentType.RemoveComponentSuffix(), EditorStyles.boldLabel);
            } else {
                debugBehaviour.unfoldedComponents[index] = EditorGUILayout.Foldout(debugBehaviour.unfoldedComponents[index], componentType.RemoveComponentSuffix(), _foldoutStyle);
            }
            if (GUILayout.Button("-", GUILayout.Width(19), GUILayout.Height(14))) {
                entity.RemoveComponent(index);
            }
            EditorGUILayout.EndHorizontal();

            if (debugBehaviour.unfoldedComponents[index]) {
                foreach (var field in fields) {
                    var value = field.GetValue(component);
                    DrawAndSetElement(field.FieldType, field.Name, value,
                        entity, index, component, newValue => field.SetValue(component, newValue));
                }
            }
            EditorGUILayout.EndVertical();
        }

        public static void DrawAndSetElement(Type type, string fieldName, object value, Entity entity, int index, IComponent component, Action<object> setValue) {
            var newValue = DrawAndGetNewValue(type, fieldName, value, entity, index, component);
            if (didValueChange(value, newValue)) {
                entity.WillRemoveComponent(index);
                setValue(newValue);
                entity.ReplaceComponent(index, component);
            }
        }

        static bool didValueChange(object value, object newValue) {
            return (value == null && newValue != null) ||
                   (value != null && newValue == null) ||
                   ((value != null && newValue != null &&
                   !newValue.Equals(value)));
        }

        public static object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            if (value == null) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(fieldName, "null");
                if (GUILayout.Button("Create", GUILayout.Height(14))) {
                    object defaultValue;
                    if (CreateDefault(type, out defaultValue)) {
                        value = defaultValue;
                    }
                }
                EditorGUILayout.EndHorizontal();
                return value;
            }

            if (!type.IsValueType) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
            }

            var typeDrawer = getTypeDrawer(type);
            if (typeDrawer != null) {
                value = typeDrawer.DrawAndGetNewValue(type, fieldName, value, entity, index, component);
            } else {
                drawUnsupportedType(type, fieldName, value);
            }

            if (!type.IsValueType) {
                EditorGUILayout.EndVertical();
                if (GUILayout.Button("x", GUILayout.Width(19), GUILayout.Height(14))) {
                    value = null;
                }
                EditorGUILayout.EndHorizontal();
            }

            return value;
        }

        static ITypeDrawer getTypeDrawer(Type type) {
            foreach (var drawer in _typeDrawers) {
                if (drawer.HandlesType(type)) {
                    return drawer;
                }
            }

            return null;
        }

        static void drawUnsupportedType(Type type, string fieldName, object value) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(fieldName, value.ToString());
            if (GUILayout.Button("Missing ITypeDrawer", GUILayout.Height(14))) {
                var typeName = TypeGenerator.Generate(type);
                if (EditorUtility.DisplayDialog(
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
            EditorGUILayout.EndHorizontal();
        }

        public static bool CreateDefault(Type type, out object defaultValue) {
            try {
                defaultValue = Activator.CreateInstance(type);
                return true;
            } catch (Exception) {
                foreach (var creator in _defaultInstanceCreators) {
                    if (creator.HandlesType(type)) {
                        defaultValue = creator.CreateDefault(type);
                        return true;
                    }
                }
            }

            var typeName = TypeGenerator.Generate(type);
            if (EditorUtility.DisplayDialog(
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

        static void generateIDefaultInstanceCreator(string typeName) {
            var config = new VisualDebuggingConfig(EntitasPreferencesEditor.LoadConfig());
            var folder = config.defaultInstanceCreatorFolderPath;
            var filePath = folder + "Default_type_InstanceCreator.cs";
            var template = string.Format(defaultInstanceCreatorTemplateFormat, typeName);
            generateTemplate(folder, filePath, template);
        }

        static void generateITypeDrawer(string typeName) {
            var config = new VisualDebuggingConfig(EntitasPreferencesEditor.LoadConfig());
            var folder = config.typeDrawerFolderPath;
            var filePath = folder + "Type_TypeDrawer.cs";
            var template = string.Format(typeDrawerTemplateFormat, typeName);
            generateTemplate(folder, filePath, template);
        }

        static void generateTemplate(string folder, string filePath, string template) {
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            File.WriteAllText(filePath, template);
            AssetDatabase.Refresh();
            EditorApplication.isPlaying = false;
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(filePath);
        }

        const string defaultInstanceCreatorTemplateFormat = @"using System;
using Entitas.Unity.VisualDebugging;

// Please rename class name and file name
public class Default_type_InstanceCreator : IDefaultInstanceCreator {{
    public bool HandlesType(Type type) {{
        return type == typeof({0});
    }}

    public object CreateDefault(Type type) {{
        // return your implementation to create an instance of type {0}
        throw new NotImplementedException();
    }}
}}
";

        const string typeDrawerTemplateFormat = @"using System;
using Entitas;
using Entitas.Unity.VisualDebugging;

public class Type_TypeDrawer : ITypeDrawer {{
    public bool HandlesType(Type type) {{
        return type == typeof({0});
    }}

    public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {{
        // return your implementation to draw the type {0}
        throw new NotImplementedException();
    }}
}}";
    }
}

