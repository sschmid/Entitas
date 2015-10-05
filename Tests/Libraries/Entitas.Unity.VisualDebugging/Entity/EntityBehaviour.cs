using System;
using Entitas;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class EntityBehaviour : MonoBehaviour {
        public Pool pool { get { return _pool; } }
        public Entity entity { get { return _entity; } }
        public string[] componentNames { get { return _componentNames; } }
        public Type[] componentTypes { get { return _componentTypes; } }
        public bool[] unfoldedComponents { get { return _unfoldedComponents; } }

        public int componentToAdd;

        Pool _pool;
        Entity _entity;
        string[] _componentNames;
        Type[] _componentTypes;
        bool[] _unfoldedComponents;

        public void Init(Pool pool, Entity entity, string[] componentNames, Type[] componentTypes) {
            _pool = pool;
            _entity = entity;
            _componentNames = componentNames;
            _componentTypes = componentTypes;
            _unfoldedComponents = new bool[_pool.totalComponents];
            _entity.OnEntityReleased += onEntityReleased;
            Update();

            UnfoldAllComponents();
        }

        public void UnfoldAllComponents() {
            for (int i = 0; i < _unfoldedComponents.Length; i++) {
                _unfoldedComponents[i] = true;
            }
        }

        public void FoldAllComponents() {
            for (int i = 0; i < _unfoldedComponents.Length; i++) {
                _unfoldedComponents[i] = false;
            }
        }

        public void DestroyBehaviour() {
            _entity.OnEntityReleased -= onEntityReleased;
            Destroy(gameObject);
        }

        void onEntityReleased(Entity e) {
            DestroyBehaviour();
        }

        void Update() {
            name = _entity.ToString();
        }
    }
}