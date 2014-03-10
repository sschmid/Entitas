using System;
using System.Collections.Generic;

namespace Entitas {
    public class Entity {
        public event EntityChange OnComponentAdded;
        public event EntityChange OnComponentRemoved;
        public event EntityChange OnComponentReplaced;

        public delegate void EntityChange(Entity entity, IComponent component);

        public ulong creationIndex { get { return _creationIndex; } }

        readonly Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();
        readonly ulong _creationIndex;

        public Entity() {
            _creationIndex = 0;
        }

        public Entity(ulong creationIndex) {
            _creationIndex = creationIndex;
        }

        public void AddComponent(IComponent component) {
            var type = component.GetType();
            if (HasComponent(type))
                throw new EntityAlreadyHasComponentException(string.Format("Cannot add {0} to {1}.", type, this), type);

            _components.Add(type, component);
            if (OnComponentAdded != null)
                OnComponentAdded(this, component);
        }

        public void RemoveComponent(Type type) {
            if (!HasComponent(type))
                throw new EntityDoesNotHaveComponentException(string.Format("Cannot remove {0} from {1}.", type, this), type);

            var component = _components[type];
            _components.Remove(type);
            if (OnComponentRemoved != null)
                OnComponentRemoved(this, component);
        }

        public void ReplaceComponent(IComponent component) {
            var type = component.GetType();
            if (HasComponent(type))
                _components.Remove(type);
            _components.Add(type, component);
            if (OnComponentReplaced != null)
                OnComponentReplaced(this, component);
        }

        public IComponent GetComponent(Type type) {
            if (!HasComponent(type))
                throw new EntityDoesNotHaveComponentException(string.Format("Cannot get {0} from {1}.", type, this), type);

            return _components[type];
        }

        public bool HasComponent(IComponent component) {
            return _components.ContainsValue(component);
        }

        public bool HasComponent(Type type) {
            return _components.ContainsKey(type);
        }

        public bool HasComponents(IEnumerable<Type> types) {
            foreach (var type in types)
                if (!_components.ContainsKey(type))
                    return false;

            return true;
        }

        public bool HasAnyComponent(IEnumerable<Type> types) {
            foreach (var type in types)
                if (_components.ContainsKey(type))
                    return true;

            return false;
        }

        public HashSet<IComponent> GetComponents() {
            return new HashSet<IComponent>(_components.Values);
        }

        public HashSet<Type> GetComponentTypes() {
            return new HashSet<Type>(_components.Keys);
        }

        public void RemoveAllComponents() {
            var types = GetComponentTypes();
            foreach (var type in types)
                RemoveComponent(type);
        }

        public override string ToString() {
            var componentsStr = string.Empty;
            var components = GetComponents();
            const string seperator = ", ";
            foreach (var component in components)
                componentsStr += component + seperator;

            if (componentsStr != string.Empty)
                componentsStr = componentsStr.Substring(0, componentsStr.Length - seperator.Length);

            return string.Format("Entity(" + componentsStr + ")");
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

