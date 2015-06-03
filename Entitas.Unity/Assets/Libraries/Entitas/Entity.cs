using System;
using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public event EntityChanged OnComponentAdded;
        public event EntityChanged OnComponentReplaced;
        public event EntityChanged OnComponentWillBeRemoved;
        public event EntityChanged OnComponentRemoved;

        public delegate void EntityChanged(Entity entity, int index, IComponent component);

        public int creationIndex { get { return _creationIndex; } }

        internal int _creationIndex;
        internal readonly IComponent[] _components;

        IComponent[] _componentsCache;
        int[] _componentIndicesCache;

        public Entity(int totalComponents) {
            _components = new IComponent[totalComponents];
        }

        public Entity AddComponent(int index, IComponent component) {
            if (HasComponent(index)) {
                var errorMsg = "Cannot add component at index " + index + " to " + this;
                throw new EntityAlreadyHasComponentException(errorMsg, index);
            }

            _components[index] = component;
            _componentsCache = null;
            _componentIndicesCache = null;
            if (OnComponentAdded != null) {
                OnComponentAdded(this, index, component);
            }

            return this;
        }

        public void WillRemoveComponent(int index) {
            if (HasComponent(index) && OnComponentWillBeRemoved != null) {
                OnComponentWillBeRemoved(this, index, _components[index]);
            }
        }

        public Entity RemoveComponent(int index) {
            if (!HasComponent(index)) {
                var errorMsg = "Cannot remove component at index " + index + " from " + this;
                throw new EntityDoesNotHaveComponentException(errorMsg, index);
            }

            removeComponent(index);

            return this;
        }

        void removeComponent(int index) {
            if (OnComponentWillBeRemoved != null) {
                OnComponentWillBeRemoved(this, index, _components[index]);
            }
            replaceComponent(index, null);
        }

        public Entity ReplaceComponent(int index, IComponent component) {
            if (HasComponent(index)) {
                replaceComponent(index, component);
            } else if (component != null) {
                AddComponent(index, component);
            }

            return this;
        }

        void replaceComponent(int index, IComponent replacement) {
            var component = _components[index];
            if (component == replacement) {
                if (OnComponentReplaced != null) {
                    OnComponentReplaced(this, index, replacement);
                }
            } else {
                _components[index] = replacement;
                _componentsCache = null;
                if (replacement == null) {
                    _componentIndicesCache = null;
                    if (OnComponentRemoved != null) {
                        OnComponentRemoved(this, index, component);
                    }
                } else if (OnComponentReplaced != null) {
                    OnComponentReplaced(this, index, replacement);
                }
            }
        }

        public IComponent GetComponent(int index) {
            if (!HasComponent(index)) {
                var errorMsg = "Cannot get component at index " + index + " from " + this;
                throw new EntityDoesNotHaveComponentException(errorMsg, index);
            }

            return _components[index];
        }

        public bool HasComponent(int index) {
            return _components[index] != null;
        }

        public bool HasComponents(int[] indices) {
            for (int i = 0, indicesLength = indices.Length; i < indicesLength; i++) {
                if (_components[indices[i]] == null) {
                    return false;
                }
            }

            return true;
        }

        public bool HasAnyComponent(int[] indices) {
            for (int i = 0, indicesLength = indices.Length; i < indicesLength; i++) {
                if (_components[indices[i]] != null) {
                    return true;
                }
            }

            return false;
        }

        public IComponent[] GetComponents() {
            if (_componentsCache == null) {
                var components = new List<IComponent>();
                for (int i = 0, componentsLength = _components.Length; i < componentsLength; i++) {
                    var component = _components[i];
                    if (component != null) {
                        components.Add(component);
                    }
                }

                _componentsCache = components.ToArray();
            }

            return _componentsCache;
        }

        public int[] GetComponentIndices() {
            if (_componentIndicesCache == null) {
                var indices = new List<int>();
                for (int i = 0, componentsLength = _components.Length; i < componentsLength; i++) {
                    if (_components[i] != null) {
                        indices.Add(i);
                    }
                }

                _componentIndicesCache = indices.ToArray();
            }

            return _componentIndicesCache;
        }

        public void RemoveAllComponents() {
            var indices = GetComponentIndices();
            for (int i = 0, indicesLength = indices.Length; i < indicesLength; i++) {
                removeComponent(indices[i]);
            }
        }

        public override string ToString() {
            const string seperator = ", ";
            var componentsStr = string.Empty;
            var components = GetComponents();
            for (int i = 0, componentsLength = components.Length; i < componentsLength; i++) {
                componentsStr += components[i] + seperator;
            }

            if (componentsStr != string.Empty) {
                componentsStr = componentsStr.Substring(0, componentsStr.Length - seperator.Length);
            }

            return string.Format("Entity_{0}({1})", _creationIndex, componentsStr);
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

    public class EntityEqualityComparer : IEqualityComparer<Entity> {

        public static readonly EntityEqualityComparer comparer = new EntityEqualityComparer();

        public bool Equals(Entity x, Entity y) {
            return x == y;
        }

        public int GetHashCode(Entity obj) {
            return obj._creationIndex;
        }
    }
}

