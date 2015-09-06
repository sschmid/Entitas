using System;
using System.Collections.Generic;
using System.Text;

namespace Entitas {

    /// <summary>
    /// The Entity class serves as the lifecycle manager for the <see cref="IComponent"/>, managing creation, caching and event handling.
    /// </summary>
    public partial class Entity {

        /// <summary>
        /// Called whenever a Component is attached to an entity that did not have a Component of that type attached to it.
        /// </summary>
        public event EntityChanged OnComponentAdded;

        /// <summary>
        /// Called whenever a Component is removed from an Entity.
        /// </summary>
        public event EntityChanged OnComponentRemoved;

        /// <summary>
        /// Called whenever a Component is replaced with a Component of the same type.
        /// </summary>
        public event ComponentReplaced OnComponentReplaced;

        public delegate void EntityChanged(Entity entity, int index, IComponent component);
        public delegate void ComponentReplaced(Entity entity, int index, IComponent previousComponent, IComponent newComponent);

        public int creationIndex { get { return _creationIndex; } }

        internal int _creationIndex;
        internal bool _isEnabled = true;
        readonly IComponent[] _components;

        IComponent[] _componentsCache;
        int[] _componentIndicesCache;
        string _toStringCache;

        public Entity(int totalComponents) {
            _components = new IComponent[totalComponents];
        }

        /// <summary>
        /// Adds the given Component to the Entity, ensuring <see cref="OnComponentAdded"/> is called. 
        /// Preferred usage is to use the generated syntax for the specific Components. (See: AddXXXX )
        /// 
        /// Throws: <para/>
        /// - <see cref="EntityIsNotEnabledException"/> if the Entity is not enabled. <para/>
        /// - <see cref="EntityAlreadyHasComponentException"/> if the Entity already has the Component. Use <see cref="ReplaceComponent"/> instead.<para/>
        /// </summary>
        /// <param name="index">The index of the Component to add. See generated *ComponentsIds of the Pool that instantiated this Entity.</param>
        /// <param name="component">The component to add.</param>
        public Entity AddComponent(int index, IComponent component) {
            if (!_isEnabled) {
                throw new EntityIsNotEnabledException("Cannot add component!");
            }

            if (HasComponent(index)) {
                var errorMsg = "Cannot add component at index " + index + " to " + this;
                throw new EntityAlreadyHasComponentException(errorMsg, index);
            }

            _components[index] = component;
            _componentsCache = null;
            _componentIndicesCache = null;
            _toStringCache = null;
            if (OnComponentAdded != null) {
                OnComponentAdded(this, index, component);
            }

            return this;
        }

        /// <summary>
        /// Removes the specified Component from the Entity, ensuring <see cref="OnComponentRemoved"/> is called. 
        /// Preferred usage is to use the generated syntax for the specific Components. (See: RemoveXXXX )
        /// 
        /// <para/>
        /// Throws: <para/>
        /// - <see cref="EntityIsNotEnabledException"/> if the Entity is not enabled. <para/>
        /// - <see cref="EntityDoesNotHaveComponentException"/> if the Entity does not have the Component.<para/>
        /// </summary>
        /// <param name="index">The index of the Component to remove. See generated *ComponentsIds of the Pool that instantiated this Entity.</param>
        /// <param name="component">The component to remove.</param>
        public Entity RemoveComponent(int index) {
            if (!_isEnabled) {
                throw new EntityIsNotEnabledException("Cannot remove component!");
            }

            if (!HasComponent(index)) {
                var errorMsg = "Cannot remove component at index " + index + " from " + this;
                throw new EntityDoesNotHaveComponentException(errorMsg, index);
            }

            replaceComponent(index, null);

            return this;
        }

        /// <summary>
        /// Replaces the specified Component from the Entity, ensuring the following lifecycle: <para/> 
        /// - If the entity does not have the Component, calls <see cref="OnComponentAdded"/>. <para/>
        /// - Otherwise if the given component is null, calls <see cref="OnComponentRemoved"/>. <para/>
        /// - Else calls <see cref="OnComponentReplaced"/>. <para/> 
        /// 
        /// Preferred usage is to use the generated syntax for the specific Components, which handles caching of the previously used Component. (See: ReplaceXXXX )
        /// 
        /// <para/>
        /// Throws: <para/>
        /// - <see cref="EntityIsNotEnabledException"/> if the Entity is not enabled. <para/>
        /// </summary>
        /// <param name="index">The index of the Component to add. See generated *ComponentsIds of the Pool that instantiated this Entity.</param>
        /// <param name="component">The component to replace.</param>
        public Entity ReplaceComponent(int index, IComponent component) {
            if (!_isEnabled) {
                throw new EntityIsNotEnabledException("Cannot replace component!");
            }

            if (HasComponent(index)) {
                replaceComponent(index, component);
            } else if (component != null) {
                AddComponent(index, component);
            }

            return this;
        }

        void replaceComponent(int index, IComponent replacement) {
            var previousComponent = _components[index];
            if (previousComponent == replacement) {
                if (OnComponentReplaced != null) {
                    OnComponentReplaced(this, index, previousComponent, replacement);
                }
            } else {
                _components[index] = replacement;
                _componentsCache = null;
                if (replacement == null) {
                    _componentIndicesCache = null;
                    _toStringCache = null;
                    if (OnComponentRemoved != null) {
                        OnComponentRemoved(this, index, previousComponent);
                    }
                } else {
                    if (OnComponentReplaced != null) {
                        OnComponentReplaced(this, index, previousComponent, replacement);
                    }
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

        public IComponent[] GetComponents() {
            if (_componentsCache == null) {
                var components = new List<IComponent>(16);
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
                var indices = new List<int>(16);
                for (int i = 0, componentsLength = _components.Length; i < componentsLength; i++) {
                    if (_components[i] != null) {
                        indices.Add(i);
                    }
                }

                _componentIndicesCache = indices.ToArray();
            }

            return _componentIndicesCache;
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

        public void RemoveAllComponents() {
            _toStringCache = null;
            for (int i = 0, componentsLength = _components.Length; i < componentsLength; i++) {
                if (_components[i] != null) {
                    replaceComponent(i, null);
                }
            }
        }

        /// <summary>
        /// Used internally by the Pool to destroy an entity.
        /// To destroy an entity, use <see cref="Pool.DestroyEntity"/> on the source Pool of this Entity.
        /// </summary>
        internal void destroy() {
            RemoveAllComponents();
            OnComponentAdded = null;
            OnComponentReplaced = null;
            OnComponentRemoved = null;
            _isEnabled = false;
        }

        public override string ToString() {
            if (_toStringCache == null) {
                var sb = new StringBuilder()
                    .Append("Entity_")
                    .Append(_creationIndex)
                    .Append("(");

                const string seperator = ", ";
                var components = GetComponents();
                var lastSeperator = components.Length - 1 ;
                for (int i = 0, componentsLength = components.Length; i < componentsLength; i++) {
                    sb.Append(components[i].GetType());
                    if (i < lastSeperator) {
                        sb.Append(seperator);
                    }
                }

                sb.Append(")");
                _toStringCache = sb.ToString();
            }

            return _toStringCache;
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

    public class EntityIsNotEnabledException : Exception {
        public EntityIsNotEnabledException(string message) :
            base(message + "\nEntity is not enabled!") {
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

    public partial class Entity {
        public event EntityReleased OnEntityReleased;
        public delegate void EntityReleased(Entity entity);

        internal int _refCount;

        /// <summary>
        /// Increases the reference count on this Entity. Used by the Entity Reference Counting.
        /// </summary>
        public Entity Retain() {
            _refCount += 1;
            return this;
        }
        
        /// <summary>
        /// Decreases the reference count on this Entity and ensures callbacks are made when nothing references this Entity.
        /// See <see cref="Pool.DestroyEntity"/> to directly destroy an entity.
        /// </summary>
        public void Release() {
            _refCount -= 1;
            if (_refCount == 0) {
                if (OnEntityReleased != null) {
                    OnEntityReleased(this);
                }
            } else if (_refCount < 0) {
                throw new EntityIsAlreadyReleasedException();
            }
        }
    }

    public class EntityIsAlreadyReleasedException : Exception {
        public EntityIsAlreadyReleasedException() :
            base("Entity is already released!") {
        }
    }
}

