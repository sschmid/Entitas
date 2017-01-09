using System;
using System.Linq;
using System.Reflection;
using Entitas.Serialization;
using Entitas.Serialization.Blueprints;
using Entitas.Unity.VisualDebugging;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Entitas.Unity.Serialization.Blueprints {

    [CustomEditor(typeof(BinaryBlueprint))]
    public class BinaryBlueprintInspector : Editor {

        public static BinaryBlueprint[] FindAllBlueprints() {
            return AssetDatabase.FindAssets("l:" + BinaryBlueprintPostprocessor.ASSET_LABEL)
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<BinaryBlueprint>(path))
                .ToArray();
        }

        [DidReloadScripts, MenuItem(EntitasMenuItems.blueprints_update_all_blueprints, false, EntitasMenuItemPriorities.blueprints_update_all_blueprints)]
        public static void UpdateAllBinaryBlueprints() {
            if(!EditorApplication.isPlayingOrWillChangePlaymode) {
                var allContexts = findAllContexts();
                if(allContexts == null) {
                    return;
                }

                var binaryBlueprints = FindAllBlueprints();
                var allContextNames = allContexts.Select(context => context.contextInfo.name).ToArray();
                var updated = 0;
                foreach(var binaryBlueprint in binaryBlueprints) {
                    var didUpdate = UpdateBinaryBlueprint(binaryBlueprint, allContexts, allContextNames);
                    if(didUpdate) {
                        updated += 1;
                    }
                }

                if(updated > 0) {
                    Debug.Log("Validated " + binaryBlueprints.Length + " Blueprints, " + updated + " have been updated.");
                }
            }
        }

        public static bool UpdateBinaryBlueprint(BinaryBlueprint binaryBlueprint, Context[] allContexts, string[] allContextNames) {
            var blueprint = binaryBlueprint.Deserialize();
            var needsUpdate = false;

            var contextIndex = Array.IndexOf(allContextNames, blueprint.contextIdentifier);
            if(contextIndex < 0) {
                contextIndex = 0;
                needsUpdate = true;
            }

            var context = allContexts[contextIndex];
            blueprint.contextIdentifier = context.contextInfo.name;

            foreach(var component in blueprint.components) {
                var type = component.fullTypeName.ToType();
                var index = Array.IndexOf(context.contextInfo.componentTypes, type);

                if(index != component.index) {
                    Debug.Log(string.Format(
                        "Blueprint '{0}' has invalid or outdated component index for '{1}'. Index was {2} but should be {3}. This will be fixed now!",
                        blueprint.name, component.fullTypeName, component.index, index));

                    component.index = index;
                    needsUpdate = true;
                }
            }

            if(needsUpdate) {
                Debug.Log("Updating Blueprint '" + blueprint.name + "'");
                binaryBlueprint.Serialize(blueprint);
            }

            return needsUpdate;
        }

        static Context[] findAllContexts() {

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            var allContextsProperty = typeof(Contexts).GetProperty("allContexts", bindingFlags);
            if(allContextsProperty != null) {
                var contextsType = typeof(Contexts);
                var setAllContextsMethod = contextsType.GetMethod("SetAllContexts", bindingFlags);
                if(setAllContextsMethod != null) {
                    var contexts = new Contexts();
                    setAllContextsMethod.Invoke(contexts, null);
                    var allContextsGetter = contextsType.GetProperty("allContexts", bindingFlags);

                    return (Context[])allContextsGetter.GetValue(contexts, null);
                }
            }

            return new Context[0];
        }

        Blueprint _blueprint;

        Context[] _allContexts;
        string[] _allContextNames;
        int _contextIndex;

        Context _context;
        Entity _entity;

        void Awake() {
            _allContexts = findAllContexts();
            if(_allContexts == null) {
                return;
            }

            var binaryBlueprint = ((BinaryBlueprint)target);

            _allContextNames = _allContexts.Select(context => context.contextInfo.name).ToArray();

            BinaryBlueprintInspector.UpdateBinaryBlueprint(binaryBlueprint, _allContexts, _allContextNames);

            _blueprint = binaryBlueprint.Deserialize();

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), _blueprint.name);

            _contextIndex = Array.IndexOf(_allContextNames, _blueprint.contextIdentifier);
            switchToContext();

            _entity.ApplyBlueprint(_blueprint);

            EntityDrawer.Initialize();
        }

        void OnDisable() {
            if(_context != null) {
                _context.Reset();
            }
        }

        public override void OnInspectorGUI() {
            var binaryBlueprint = ((BinaryBlueprint)target);

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.LabelField("Blueprint", EditorStyles.boldLabel);
                binaryBlueprint.name = EditorGUILayout.TextField("Name", binaryBlueprint.name);

                EntitasEditorLayout.BeginHorizontal();
                {
                    _contextIndex = EditorGUILayout.Popup(_contextIndex, _allContextNames);

                    if(GUILayout.Button("Switch Context")) {
                        switchToContext();
                    }
                }
                EntitasEditorLayout.EndHorizontal();

                EntityDrawer.DrawComponents(_context, _entity, true);
            }
            var changed = EditorGUI.EndChangeCheck();
            if(changed) {
                binaryBlueprint.Serialize(_entity);
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), binaryBlueprint.name);
                EditorUtility.SetDirty(target);
            }
        }

        void switchToContext() {
            if(_context != null) {
                _context.Reset();
            }
            var targetContext = _allContexts[_contextIndex];
            _context = new Context(targetContext.totalComponents, 0, targetContext.contextInfo);
            _entity = _context.CreateEntity();
        }
    }
}
