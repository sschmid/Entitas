using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    [InitializeOnLoad]
    public static class EntitasHierarchyIcon {

        static Texture2D poolHierarchyIcon {
            get {
                if (_poolHierarchyIcon == null) {
                    _poolHierarchyIcon = EntitasEditorLayout.LoadTexture("l:PoolHierarchyIcon");
                }
                return _poolHierarchyIcon;
            }
        }

        static Texture2D entityHierarchyIcon {
            get {
                if (_entityhierarchyIcon == null) {
                    _entityhierarchyIcon = EntitasEditorLayout.LoadTexture("l:EntityHierarchyIcon");
                }
                return _entityhierarchyIcon;
            }
        }

        static Texture2D systemsHierarchyIcon {
            get {
                if (_systemsHierarchyIcon == null) {
                    _systemsHierarchyIcon = EntitasEditorLayout.LoadTexture("l:SystemsHierarchyIcon");
                }
                return _systemsHierarchyIcon;
            }
        }

        static Texture2D _poolHierarchyIcon;
        static Texture2D _entityhierarchyIcon;
        static Texture2D _systemsHierarchyIcon;

        static EntitasHierarchyIcon() {
            EditorApplication.hierarchyWindowItemOnGUI += onHierarchyWindowItemOnGUI;
        }

        static void onHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect) {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject != null) {
                const float iconSize = 16f;
                const float iconOffset = iconSize + 2f;
                var rect = new Rect(selectionRect.x + selectionRect.width - iconOffset, selectionRect.y, iconSize, iconSize);

                if (gameObject.GetComponent<PoolObserverBehaviour>() != null) {
                    GUI.DrawTexture(rect, poolHierarchyIcon);
                }
                if (gameObject.GetComponent<EntityBehaviour>() != null) {
                    GUI.DrawTexture(rect, entityHierarchyIcon);
                }
                if (gameObject.GetComponent<DebugSystemsBehaviour>() != null) {
                    GUI.DrawTexture(rect, systemsHierarchyIcon);
                }
            }
        }
    }
}