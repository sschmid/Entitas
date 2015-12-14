using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    [InitializeOnLoad]
    public static class EntitasHierarchyIcon {
        static Texture2D hierarchyIcon {
            get {
                if (_hierarchyIcon == null) {
                    var guid = AssetDatabase.FindAssets("l:Entitas-HierarchyIcon")[0];
                    if (guid != null) {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        _hierarchyIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    }

                }
                return _hierarchyIcon;
            }
        }

        static Texture2D _hierarchyIcon;

        static EntitasHierarchyIcon() {
            EditorApplication.hierarchyWindowItemOnGUI += onHierarchyWindowItemOnGUI;
        }

        static void onHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect) {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject != null) {
                if (gameObject.GetComponent<PoolObserverBehaviour>() != null
                    || gameObject.GetComponent<EntityBehaviour>() != null
                    || gameObject.GetComponent<DebugSystemsBehaviour>() != null) {

                    const float ICON_SIZE = 16f;
                    const float ICON_OFFSET = ICON_SIZE + 2f;
                    var rect = new Rect(selectionRect.x + selectionRect.width - ICON_OFFSET, selectionRect.y, ICON_SIZE, ICON_SIZE);
                    GUI.DrawTexture(rect, hierarchyIcon);
                }
            }
        }
    }
}