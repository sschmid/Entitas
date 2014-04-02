using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas {
    public class Entity {
        public event EntityChange OnComponentAdded;
        public event EntityChange OnComponentRemoved;
        public event EntityChange OnComponentReplaced;

        public delegate void EntityChange(Entity entity, IComponent component);

        public ulong creationIndex { get { return _creationIndex; } }

        readonly Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();
        readonly ulong _creationIndex;
        IComponent[] _componentsCache;
        Type[] _componentTypesCache;

        public Entity() {
            _creationIndex = 0;
        }

        public Entity(ulong creationIndex) {
            _creationIndex = creationIndex;
        }

        public void AddComponent(IComponent component) {
            var type = component.GetType();

            #if DEBUG
            if (HasComponent(type))
                throw new EntityAlreadyHasComponentException(string.Format("Cannot add {0} to {1}.", type, this), type);
            #endif

            _components.Add(type, component);
            _componentsCache = null;
            _componentTypesCache = null;
            if (OnComponentAdded != null)
                OnComponentAdded(this, component);
        }

        public void RemoveComponent(Type type) {
            #if DEBUG
            if (!HasComponent(type))
                throw new EntityDoesNotHaveComponentException(string.Format("Cannot remove {0} from {1}.", type, this), type);
            #endif

            var component = _components[type];
            _components.Remove(type);
            _componentsCache = null;
            _componentTypesCache = null;
            if (OnComponentRemoved != null)
                OnComponentRemoved(this, component);
        }

        public void ReplaceComponent(IComponent component) {
            var type = component.GetType();
            if (HasComponent(type)) {
                if (_components[type] != component) {
                    _components[type] = component;
                    _componentsCache = null;
                }
                if (OnComponentReplaced != null)
                    OnComponentReplaced(this, component);
            } else {
                AddComponent(component);
            }
        }

        public IComponent GetComponent(Type type) {
            #if DEBUG
            if (!HasComponent(type))
                throw new EntityDoesNotHaveComponentException(string.Format("Cannot get {0} from {1}.", type, this), type);
            #endif

            return _components[type];
        }

        public bool HasComponent(IComponent component) {
            return _components.ContainsValue(component);
        }

        public bool HasComponent(Type type) {
            return _components.ContainsKey(type);
        }

        public bool HasComponents(Type[] types) {
            foreach (var type in types)
                if (!_components.ContainsKey(type))
                    return false;

            return true;
        }

        public bool HasAnyComponent(Type[] types) {
            foreach (var type in types)
                if (_components.ContainsKey(type))
                    return true;

            return false;
        }

        public IComponent[] GetComponents() {
            if (_componentsCache == null)
                _componentsCache = _components.Values.ToArray();

            return _componentsCache;
        }

        public Type[] GetComponentTypes() {
            if (_componentTypesCache == null)
                _componentTypesCache = _components.Keys.ToArray();

            return _componentTypesCache;
        }

        public void RemoveAllComponents() {
            var types = GetComponentTypes();
            foreach (var type in types)
                RemoveComponent(type);
        }

        public override string ToString() {
            const string seperator = ", ";
            var componentsStr = string.Empty;
            var components = GetComponents();
            foreach (var component in components)
                componentsStr += component + seperator;

            if (componentsStr != string.Empty)
                componentsStr = componentsStr.Substring(0, componentsStr.Length - seperator.Length);

            return string.Format("Entity({0})", componentsStr);
        }
    }

    public class EntityAlreadyHasComponentException : Exception {
        public EntityAlreadyHasComponentException(string message, Type type) :
            base(message + "\nEntity already has a component of type " + type) {
        }
    }

    public class EntityDoesNotHaveComponentException : Exception {
        public EntityDoesNotHaveComponentException(string message, Type type) :
            base(message + "\nEntity does not have a component of type " + type) {
        }
    }
}

