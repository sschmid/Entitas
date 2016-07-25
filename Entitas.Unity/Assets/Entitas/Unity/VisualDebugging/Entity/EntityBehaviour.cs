using Entitas;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    [ExecuteInEditMode]
    public class EntityBehaviour : MonoBehaviour {
        public Pool pool { get { return _pool; } }

        public Entity entity { get { return _entity; } }

        Pool _pool;
        Entity _entity;
        string _cachedName;

        public void Init(Pool pool, Entity entity) {
            _pool = pool;
            _entity = entity;
            _entity.OnEntityReleased += onEntityReleased;
            UpdateName();
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGui;
        }

        void onEntityReleased(Entity e) {
            gameObject.DestroyGameObject();
        }

        void UpdateName() {
            if (_entity != null && _cachedName != _entity.ToString()) {
                name = _cachedName = _entity.ToString();
            }
        }

        void HierarchyWindowItemOnGui(int instanceID, Rect selectionRect) {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject != this.gameObject) {
                UpdateName();
            }
        }

        void OnDestroy() {
            if (_entity != null) {
                _entity.OnEntityReleased -= onEntityReleased;
            }
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemOnGui;
        }
    }
}