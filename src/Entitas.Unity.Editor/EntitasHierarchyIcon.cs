using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    [InitializeOnLoad]
    public static class EntitasHierarchyIcon
    {
        static Texture2D ContextHierarchyIcon
        {
            get
            {
                if (_contextHierarchyIcon == null)
                    _contextHierarchyIcon = EditorLayout.LoadTexture("l:EntitasContextHierarchyIcon");

                return _contextHierarchyIcon;
            }
        }

        static Texture2D ContextErrorHierarchyIcon
        {
            get
            {
                if (_contextErrorHierarchyIcon == null)
                    _contextErrorHierarchyIcon = EditorLayout.LoadTexture("l:EntitasContextErrorHierarchyIcon");

                return _contextErrorHierarchyIcon;
            }
        }

        static Texture2D EntityHierarchyIcon
        {
            get
            {
                if (_entityHierarchyIcon == null)
                    _entityHierarchyIcon = EditorLayout.LoadTexture("l:EntitasEntityHierarchyIcon");

                return _entityHierarchyIcon;
            }
        }

        static Texture2D EntityErrorHierarchyIcon
        {
            get
            {
                if (_entityErrorHierarchyIcon == null)
                    _entityErrorHierarchyIcon = EditorLayout.LoadTexture("l:EntitasEntityErrorHierarchyIcon");

                return _entityErrorHierarchyIcon;
            }
        }

        static Texture2D EntityLinkHierarchyIcon
        {
            get
            {
                if (_entityLinkHierarchyIcon == null)
                    _entityLinkHierarchyIcon = EditorLayout.LoadTexture("l:EntitasEntityLinkHierarchyIcon");

                return _entityLinkHierarchyIcon;
            }
        }

        static Texture2D EntityLinkWarnHierarchyIcon
        {
            get
            {
                if (_entityLinkWarnHierarchyIcon == null)
                    _entityLinkWarnHierarchyIcon = EditorLayout.LoadTexture("l:EntitasEntityLinkWarnHierarchyIcon");

                return _entityLinkWarnHierarchyIcon;
            }
        }

        static Texture2D SystemsHierarchyIcon
        {
            get
            {
                if (_systemsHierarchyIcon == null)
                    _systemsHierarchyIcon = EditorLayout.LoadTexture("l:EntitasSystemsHierarchyIcon");

                return _systemsHierarchyIcon;
            }
        }

        static Texture2D SystemsWarnHierarchyIcon
        {
            get
            {
                if (_systemsWarnHierarchyIcon == null)
                    _systemsWarnHierarchyIcon = EditorLayout.LoadTexture("l:EntitasSystemsWarnHierarchyIcon");

                return _systemsWarnHierarchyIcon;
            }
        }

        static Texture2D _contextHierarchyIcon;
        static Texture2D _contextErrorHierarchyIcon;
        static Texture2D _entityHierarchyIcon;
        static Texture2D _entityErrorHierarchyIcon;
        static Texture2D _entityLinkHierarchyIcon;
        static Texture2D _entityLinkWarnHierarchyIcon;
        static Texture2D _systemsHierarchyIcon;
        static Texture2D _systemsWarnHierarchyIcon;

        static readonly int SystemWarningThreshold;

        static EntitasHierarchyIcon()
        {
            SystemWarningThreshold = EntitasSettings.Instance.SystemWarningThreshold;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }

        static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null)
                return;

            const float iconSize = 16f;
            const float iconOffset = iconSize + 2f;
            var rect = new Rect(selectionRect.x + selectionRect.width - iconOffset, selectionRect.y, iconSize, iconSize);

            if (gameObject.TryGetComponent<ContextObserverBehaviour>(out var contextObserver))
            {
                GUI.DrawTexture(rect, contextObserver.Context.RetainedEntitiesCount != 0
                    ? ContextErrorHierarchyIcon
                    : ContextHierarchyIcon);

                return;
            }

            if (gameObject.TryGetComponent<EntityBehaviour>(out var entityBehaviour))
            {
                GUI.DrawTexture(rect, entityBehaviour.Entity.IsEnabled
                    ? EntityHierarchyIcon
                    : EntityErrorHierarchyIcon);
                return;
            }

            if (gameObject.TryGetComponent<EntityLink>(out var entityLink))
            {
                GUI.DrawTexture(rect, entityLink.Entity != null
                    ? EntityLinkHierarchyIcon
                    : EntityLinkWarnHierarchyIcon);

                return;
            }

            if (gameObject.TryGetComponent<DebugSystemsBehaviour>(out var debugSystemsBehaviour))
            {
                GUI.DrawTexture(rect, debugSystemsBehaviour.Systems.ExecuteDuration < SystemWarningThreshold
                    ? SystemsHierarchyIcon
                    : SystemsWarnHierarchyIcon);

                return;
            }
        }
    }
}
