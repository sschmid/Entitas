using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Entitas;
using Entitas.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    [CustomEditor(typeof(EntityBehaviour)), CanEditMultipleObjects]
    public class EntityInspector : Editor {
        GUIStyle _foldoutStyle;
        static IDefaultInstanceCreator[] _defaultInstanceCreators;
        static ITypeDrawer[] _typeDrawers;

        void Awake() {
            _foldoutStyle = new GUIStyle(EditorStyles.foldout);
            _foldoutStyle.fontStyle = FontStyle.Bold;

            var types = Assembly.GetAssembly(typeof(EntityInspector)).GetTypes();

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
            var entityBehaviour = (EntityBehaviour)target;
            var pool = entityBehaviour.pool;
            var entity = entityBehaviour.entity;

            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Destroy Entity")) {
                entityBehaviour.DestroyBehaviour();
                pool.DestroyEntity(entity);
            }
            GUI.backgroundColor = bgColor;

            EntitasEditorLayout.BeginVerticalBox();
            {
                EntitasEditorLayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Components (" + entity.GetComponents().Length + ")", EditorStyles.boldLabel);
                    if (GUILayout.Button("▸", GUILayout.Width(21), GUILayout.Height(14))) {
                        entityBehaviour.FoldAllComponents();
                    }
                    if (GUILayout.Button("▾", GUILayout.Width(21), GUILayout.Height(14))) {
                        entityBehaviour.UnfoldAllComponents();
                    }
                }
                EntitasEditorLayout.EndHorizontal();

                EditorGUILayout.Space();
                entityBehaviour.componentToAdd = EditorGUILayout.Popup("Add Component", entityBehaviour.componentToAdd, entityBehaviour.entity.poolMetaData.componentNames);
                if (entityBehaviour.componentToAdd >= 0) {
                    var index = entityBehaviour.componentToAdd;
                    entityBehaviour.componentToAdd = -1;
                    var componentType = entityBehaviour.componentTypes[index];
                    var component = (IComponent)Activator.CreateInstance(componentType);
                    entity.AddComponent(index, component);
                }

                EditorGUILayout.Space();
                var indices = entity.GetComponentIndices();
                var components = entity.GetComponents();
                for (int i = 0; i < components.Length; i++) {
                    drawComponent(entityBehaviour, entity, indices[i], components[i]);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Retained by (" + entity.retainCount + ")", EditorStyles.boldLabel);

                #if !ENTITAS_FAST_AND_UNSAFE

                EntitasEditorLayout.BeginVerticalBox();
                {
                    foreach (var owner in entity.owners.ToArray()) {
                        EntitasEditorLayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(owner.ToString());
                            if (GUILayout.Button("Release", GUILayout.Width(88), GUILayout.Height(14))) {
                                entity.Release(owner);
                            }
                            EntitasEditorLayout.EndHorizontal();
                        }
                    }
                }
                EntitasEditorLayout.EndVertical();

                #endif
            }
            EntitasEditorLayout.EndVertical();
        }

        void drawMultiTargets() {
            EditorGUILayout.Space();
            EntitasEditorLayout.BeginHorizontal();
            {
                var aEntityBehaviour = (EntityBehaviour)targets[0];
                aEntityBehaviour.componentToAdd = EditorGUILayout.Popup("Add Component", aEntityBehaviour.componentToAdd, aEntityBehaviour.entity.poolMetaData.componentNames);
                if (aEntityBehaviour.componentToAdd >= 0) {
                    var index = aEntityBehaviour.componentToAdd;
                    aEntityBehaviour.componentToAdd = -1;
                    var componentType = aEntityBehaviour.componentTypes[index];
                    foreach (var t in targets) {
                        var entity = ((EntityBehaviour)t).entity;
                        var component = (IComponent)Activator.CreateInstance(componentType);
                        entity.AddComponent(index, component);
                    }
                }
            }
            EntitasEditorLayout.EndHorizontal();

            EditorGUILayout.Space();
            if (GUILayout.Button("Destroy selected entities")) {
                foreach (var t in targets) {
                    var entityBehaviour = (EntityBehaviour)t;
                    var pool = entityBehaviour.pool;
                    var entity = entityBehaviour.entity;
                    entityBehaviour.DestroyBehaviour();
                    pool.DestroyEntity(entity);
                }
            }

            EditorGUILayout.Space();

            foreach (var t in targets) {
                var entityBehaviour = (EntityBehaviour)t;
                var pool = entityBehaviour.pool;
                var entity = entityBehaviour.entity;

                EntitasEditorLayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(entity.ToString());
                    if (GUILayout.Button("Destroy Entity")) {
                        entityBehaviour.DestroyBehaviour();
                        pool.DestroyEntity(entity);
                    }
                }
                EntitasEditorLayout.EndHorizontal();
            }
        }

        void drawComponent(EntityBehaviour entityBehaviour, Entity entity, int index, IComponent component) {
            var componentType = component.GetType();
            var fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            var hue = (float)index / (float)entity.totalComponents;
            var componentColor = Color.HSVToRGB(hue, 0.7f, 1f);

            EntitasEditorLayout.BeginVerticalBox(componentColor);
            {
                EntitasEditorLayout.BeginHorizontal();
                {
                    if (fields.Length == 0) {
                        EditorGUILayout.LabelField(componentType.Name.RemoveComponentSuffix(), EditorStyles.boldLabel);
                    } else {
                        entityBehaviour.unfoldedComponents[index] = EditorGUILayout.Foldout(entityBehaviour.unfoldedComponents[index], componentType.Name.RemoveComponentSuffix(), _foldoutStyle);
                    }
                    if (GUILayout.Button("-", GUILayout.Width(19), GUILayout.Height(14))) {
                        entity.RemoveComponent(index);
                    }
                }
                EntitasEditorLayout.EndHorizontal();

                if (entityBehaviour.unfoldedComponents[index]) {
                    foreach (var field in fields) {
                        var value = field.GetValue(component);
                        DrawAndSetElement(field.FieldType, field.Name, value,
                            entity, index, component, newValue => field.SetValue(component, newValue));
                    }
                }
            }
            EntitasEditorLayout.EndVertical();
        }

        public static void DrawAndSetElement(Type type, string fieldName, object value, Entity entity, int index, IComponent component, Action<object> setValue) {
            var newValue = DrawAndGetNewValue(type, fieldName, value, entity, index, component);
            if (DidValueChange(value, newValue)) {
                setValue(newValue);
                entity.ReplaceComponent(index, component);
            }
        }

        public static bool DidValueChange(object value, object newValue) {
            return (value == null && newValue != null) ||
            (value != null && newValue == null) ||
            ((value != null && newValue != null &&
            !newValue.Equals(value)));
        }

        public static object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
            if (value == null) {
                var isUnityObject = type == typeof(UnityEngine.Object) || type.IsSubclassOf(typeof(UnityEngine.Object));
                EntitasEditorLayout.BeginHorizontal();
                {
                    if (isUnityObject) {
                        value = EditorGUILayout.ObjectField(fieldName, (UnityEngine.Object)value, type, true);
                    } else {
                        EditorGUILayout.LabelField(fieldName, "null");
                    }

                    if (GUILayout.Button("Create", GUILayout.Height(14))) {
                        object defaultValue;
                        if (CreateDefault(type, out defaultValue)) {
                            value = defaultValue;
                        }
                    }
                }
                EntitasEditorLayout.EndHorizontal();
                return value;
            }

            if (!type.IsValueType) {
                EntitasEditorLayout.BeginHorizontal();
                EntitasEditorLayout.BeginVertical();
            }

            var typeDrawer = getTypeDrawer(type);
            if (typeDrawer != null) {
                value = typeDrawer.DrawAndGetNewValue(type, fieldName, value, entity, index, component);
            } else {
                drawUnsupportedType(type, fieldName, value);
            }

            if (!type.IsValueType) {
                EntitasEditorLayout.EndVertical();
                if (GUILayout.Button("x", GUILayout.Width(19), GUILayout.Height(14))) {
                    value = null;
                }
                EntitasEditorLayout.EndHorizontal();
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
            EntitasEditorLayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(fieldName, value.ToString());
                if (GUILayout.Button("Missing ITypeDrawer", GUILayout.Height(14))) {
                    var typeName = type.ToCompilableString();
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
            }
            EntitasEditorLayout.EndHorizontal();
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

            var typeName = type.ToCompilableString();
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
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            File.WriteAllText(filePath, template);
            AssetDatabase.Refresh();
            EditorApplication.isPlaying = false;
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(filePath);
        }

        const string DEFAULT_INSTANCE_CREATOR_TEMPLATE_FORMAT = @"using System;
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

        const string TYPE_DRAWER_TEMPLATE_FORMAT = @"using System;
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
