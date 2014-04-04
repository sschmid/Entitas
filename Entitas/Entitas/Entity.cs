using System;
using System.Collections.Generic;

namespace Entitas {
    public class Entity {
        public event EntityChange OnComponentAdded;
        public event EntityChange OnComponentRemoved;
        public event EntityChange OnComponentReplaced;

        public delegate void EntityChange(Entity entity, int index, IComponent component);

        public ulong creationIndex { get { return _creationIndex; } }

        readonly IComponent[] _components;
        readonly ulong _creationIndex;
        IComponent[] _componentsCache;
        int[] _componentIndicesCache;

        public Entity(int numComponents) {
            _creationIndex = 0;
            _components = new IComponent[numComponents];
        }

        public Entity(int numComponents, ulong creationIndex) {
            _creationIndex = creationIndex;
            _components = new IComponent[numComponents];
        }

        public void AddComponent(int index, IComponent component) {
            #if (DEBUG)
            if (HasComponent(index))
                throw new EntityAlreadyHasComponentException(string.Format("Cannot add component at index {0} to {1}.", index, this), index);
            #endif

            _components[index] = component;
            _componentsCache = null;
            _componentIndicesCache = null;
            if (OnComponentAdded != null)
                OnComponentAdded(this, index, component);
        }

        public void RemoveComponent(int index) {
            #if (DEBUG)
            if (!HasComponent(index))
                throw new EntityDoesNotHaveComponentException(string.Format("Cannot remove component at index {0} from {1}.", index, this), index);
            #endif

            var component = _components[index];
            _components[index] = null;
            _componentsCache = null;
            _componentIndicesCache = null;
            if (OnComponentRemoved != null)
                OnComponentRemoved(this, index, component);
        }

        public void ReplaceComponent(int index, IComponent component) {
            if (HasComponent(index)) {
                if (_components[index] != component) {
                    _components[index] = component;
                    _componentsCache = null;
                }
                if (OnComponentReplaced != null)
                    OnComponentReplaced(this, index, component);
            } else {
                AddComponent(index, component);
            }
        }

        public IComponent GetComponent(int index) {
            #if (DEBUG)
            if (!HasComponent(index))
                throw new EntityDoesNotHaveComponentException(string.Format("Cannot get component at index {0} from {1}.", index, this), index);
            #endif

            return _components[index];
        }

        public bool HasComponent(int index) {
            return _components[index] != null;
        }

        public bool HasComponents(int[] indices) {
            foreach (var index in indices)
                if (_components[index] == null)
                    return false;

            return true;
        }

        public bool HasAnyComponent(int[] indices) {
            foreach (var index in indices)
                if (_components[index] != null)
                    return true;

            return false;
        }

        public IComponent[] GetComponents() {
            if (_componentsCache == null) {
                var components = new List<IComponent>();
                foreach (var component in _components)
                    if (component != null)
                        components.Add(component);

                _componentsCache = components.ToArray();
            }

            return _componentsCache;
        }

        public int[] GetComponentIndices() {
            if (_componentIndicesCache == null) {
                var indices = new List<int>();
                for (int i = 0; i < _components.Length; i++)
                    if (_components[i] != null)
                        indices.Add(i);

                _componentIndicesCache = indices.ToArray();
            }

            return _componentIndicesCache;
        }

        public void RemoveAllComponents() {
            var indices = GetComponentIndices();
            foreach (var index in indices)
                RemoveComponent(index);
        }

        public override string ToString() {
            const string seperator = ", ";
            var indicesStr = string.Empty;
            var indices = GetComponents();
            foreach (var index in indices)
                indicesStr += index + seperator;

            if (indicesStr != string.Empty)
                indicesStr = indicesStr.Substring(0, indicesStr.Length - seperator.Length);

            return string.Format("Entity({0})", indicesStr);
        }
    }

    public class EntityAlreadyHasComponentException : Exception {
        public EntityAlreadyHasComponentException(string message, int index) :
            base(message + "\nEntity already has a component at index " + index) {
        }
    }

    public class EntityDoesNotHaveComponentException : Exception {
        public EntityDoesNotHaveComponentException(string message, int index) :
            base(message + "\nEntity does not have a component at index " + index) {
        }
    }
}

