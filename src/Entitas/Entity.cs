﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Entitas
{
    /// Use context.CreateEntity() to create a new entity and
    /// entity.Destroy() to destroy it.
    /// You can add, replace and remove IComponent to an entity.
    public class Entity : IEntity
    {
        /// Occurs when a component gets added.
        /// All event handlers will be removed when
        /// the entity gets destroyed by the context.
        public event EntityComponentChanged OnComponentAdded;

        /// Occurs when a component gets removed.
        /// All event handlers will be removed when
        /// the entity gets destroyed by the context.
        public event EntityComponentChanged OnComponentRemoved;

        /// Occurs when a component gets replaced.
        /// All event handlers will be removed when
        /// the entity gets destroyed by the context.
        public event EntityComponentReplaced OnComponentReplaced;

        /// Occurs when an entity gets released and is not retained anymore.
        /// All event handlers will be removed when
        /// the entity gets destroyed by the context.
        public event EntityEvent OnEntityReleased;

        /// Occurs when calling entity.Destroy().
        /// All event handlers will be removed when
        /// the entity gets destroyed by the context.
        public event EntityEvent OnDestroyEntity;

        /// The total amount of components an entity can possibly have.
        public int TotalComponents => _totalComponents;

        /// Each entity has its own unique creationIndex which will be set by
        /// the context when you create the entity.
        public int CreationIndex => _creationIndex;

        /// The context manages the state of an entity.
        /// Active entities are enabled, destroyed entities are not.
        public bool IsEnabled => _isEnabled;

        /// ComponentPools is set by the context which created the entity and
        /// is used to reuse removed components.
        /// Removed components will be pushed to the ComponentPool.
        /// Use entity.CreateComponent(index, type) to get a new or
        /// reusable component from the ComponentPool.
        /// Use entity.GetComponentPool(index) to get a ComponentPool for
        /// a specific component index.
        public Stack<IComponent>[] ComponentPools => _componentPools;

        /// The contextInfo is set by the context which created the entity and
        /// contains information about the context.
        /// It's used to provide better error messages.
        public ContextInfo ContextInfo => _contextInfo;

        /// Automatic Entity Reference Counting (AERC)
        /// is used internally to prevent pooling retained entities.
        /// If you use retain manually you also have to
        /// release it manually at some point.
        public IAERC Aerc => _aerc;

        readonly List<IComponent> _componentBuffer = new List<IComponent>();
        readonly List<int> _indexBuffer = new List<int>();

        int _creationIndex;
        bool _isEnabled;

        int _totalComponents;
        IComponent[] _components;
        Stack<IComponent>[] _componentPools;
        ContextInfo _contextInfo;
        IAERC _aerc;

        IComponent[] _componentsCache;
        int[] _componentIndexesCache;
        string _toStringCache;
        StringBuilder _toStringBuilder;

        public void Initialize(int creationIndex, int totalComponents, Stack<IComponent>[] componentPools, ContextInfo contextInfo = null, IAERC aerc = null)
        {
            _totalComponents = totalComponents;
            _components = new IComponent[totalComponents];
            _componentPools = componentPools;

            _contextInfo = contextInfo ?? CreateDefaultContextInfo();
            _aerc = aerc ?? new SafeAERC(this);

            Reactivate(creationIndex);
        }

        ContextInfo CreateDefaultContextInfo()
        {
            var componentNames = new string[TotalComponents];
            for (var i = 0; i < componentNames.Length; i++)
                componentNames[i] = i.ToString();

            return new ContextInfo("No Context", componentNames, null);
        }

        public void Reactivate(int creationIndex)
        {
            _creationIndex = creationIndex;
            _isEnabled = true;
        }

        /// Adds a component at the specified index.
        /// You can only have one component at an index.
        /// Each component type must have its own constant index.
        /// The preferred way is to use the
        /// generated methods from the code generator.
        public void AddComponent(int index, IComponent component)
        {
            if (!_isEnabled)
                throw new EntityIsNotEnabledException($"Cannot add component '{_contextInfo.ComponentNames[index]}' to {this}!");

            if (HasComponent(index))
                throw new EntityAlreadyHasComponentException(index,
                    $"Cannot add component '{_contextInfo.ComponentNames[index]}' to {this}!",
                    "You should check if an entity already has the component before adding it or use entity.ReplaceComponent()."
                );

            _components[index] = component;
            _componentsCache = null;
            _componentIndexesCache = null;
            _toStringCache = null;
            OnComponentAdded?.Invoke(this, index, component);
        }

        /// Removes a component at the specified index.
        /// You can only remove a component at an index if it exists.
        /// The preferred way is to use the
        /// generated methods from the code generator.
        public void RemoveComponent(int index)
        {
            if (!_isEnabled)
                throw new EntityIsNotEnabledException($"Cannot remove component '{_contextInfo.ComponentNames[index]}' from {this}!");

            if (!HasComponent(index))
                throw new EntityDoesNotHaveComponentException(index,
                    $"Cannot remove component '{_contextInfo.ComponentNames[index]}' from {this}!",
                    "You should check if an entity has the component before removing it.");

            ReplaceComponentInternal(index, null);
        }

        /// Replaces an existing component at the specified index
        /// or adds it if it doesn't exist yet.
        /// The preferred way is to use the
        /// generated methods from the code generator.
        public void ReplaceComponent(int index, IComponent component)
        {
            if (!_isEnabled)
                throw new EntityIsNotEnabledException($"Cannot replace component '{_contextInfo.ComponentNames[index]}' on {this}!");

            if (HasComponent(index))
                ReplaceComponentInternal(index, component);
            else if (component != null)
                AddComponent(index, component);
        }

        void ReplaceComponentInternal(int index, IComponent replacement)
        {
            // TODO VD PERFORMANCE
            // _toStringCache = null;

            var previousComponent = _components[index];
            if (replacement != previousComponent)
            {
                _components[index] = replacement;
                _componentsCache = null;
                if (replacement != null)
                {
                    OnComponentReplaced?.Invoke(this, index, previousComponent, replacement);
                }
                else
                {
                    _componentIndexesCache = null;

                    // TODO VD PERFORMANCE
                    _toStringCache = null;

                    OnComponentRemoved?.Invoke(this, index, previousComponent);
                }

                GetComponentPool(index).Push(previousComponent);
            }
            else
            {
                OnComponentReplaced?.Invoke(this, index, previousComponent, replacement);
            }
        }

        /// Returns a component at the specified index.
        /// You can only get a component at an index if it exists.
        /// The preferred way is to use the
        /// generated methods from the code generator.
        public IComponent GetComponent(int index)
        {
            if (!HasComponent(index))
                throw new EntityDoesNotHaveComponentException(index,
                    $"Cannot get component '{_contextInfo.ComponentNames[index]}' from {this}!",
                    "You should check if an entity has the component before getting it.");

            return _components[index];
        }

        /// Returns all added components.
        public IComponent[] GetComponents()
        {
            if (_componentsCache == null)
            {
                foreach (var component in _components)
                {
                    if (component != null)
                        _componentBuffer.Add(component);
                }

                _componentsCache = _componentBuffer.ToArray();
                _componentBuffer.Clear();
            }

            return _componentsCache;
        }

        /// Returns all indexes of added components.
        public int[] GetComponentIndexes()
        {
            if (_componentIndexesCache == null)
            {
                for (var i = 0; i < _components.Length; i++)
                    if (_components[i] != null)
                        _indexBuffer.Add(i);

                _componentIndexesCache = _indexBuffer.ToArray();
                _indexBuffer.Clear();
            }

            return _componentIndexesCache;
        }

        /// Determines whether this entity has a component
        /// at the specified index.
        public bool HasComponent(int index) => _components[index] != null;

        /// Determines whether this entity has components
        /// at all the specified indexes.
        public bool HasComponents(int[] indexes)
        {
            foreach (var index in indexes)
                if (_components[index] == null)
                    return false;

            return true;
        }

        /// Determines whether this entity has a component
        /// at any of the specified indexes.
        public bool HasAnyComponent(int[] indexes)
        {
            foreach (var index in indexes)
                if (_components[index] != null)
                    return true;

            return false;
        }

        /// Removes all components.
        public void RemoveAllComponents()
        {
            _toStringCache = null;
            for (var i = 0; i < _components.Length; i++)
                if (_components[i] != null)
                    ReplaceComponentInternal(i, null);
        }

        /// Returns the ComponentPool for the specified component index.
        /// ComponentPools is set by the context which created the entity and
        /// is used to reuse removed components.
        /// Removed components will be pushed to the ComponentPool.
        /// Use entity.CreateComponent(index, type) to get a new or
        /// reusable component from the ComponentPool.
        public Stack<IComponent> GetComponentPool(int index)
        {
            var componentPool = _componentPools[index];
            if (componentPool == null)
            {
                componentPool = new Stack<IComponent>();
                _componentPools[index] = componentPool;
            }

            return componentPool;
        }

        /// Returns a new or reusable component from the ComponentPool
        /// for the specified component index.
        public IComponent CreateComponent(int index, Type type)
        {
            var componentPool = GetComponentPool(index);
            return componentPool.Count > 0
                ? componentPool.Pop()
                : (IComponent)Activator.CreateInstance(type);
        }

        /// Returns a new or reusable component from the ComponentPool
        /// for the specified component index.
        public T CreateComponent<T>(int index) where T : new()
        {
            var componentPool = GetComponentPool(index);
            return componentPool.Count > 0 ? (T)componentPool.Pop() : new T();
        }

        /// Returns the number of objects that retain this entity.
        public int RetainCount => _aerc.RetainCount;

        /// Retains the entity. An owner can only retain the same entity once.
        /// Retain/Release is part of AERC (Automatic Entity Reference Counting)
        /// and is used internally to prevent pooling retained entities.
        /// If you use retain manually you also have to
        /// release it manually at some point.
        public void Retain(object owner)
        {
            _aerc.Retain(owner);

            // TODO VD PERFORMANCE
            // _toStringCache = null;
        }

        /// Releases the entity. An owner can only release an entity
        /// if it retains it.
        /// Retain/Release is part of AERC (Automatic Entity Reference Counting)
        /// and is used internally to prevent pooling retained entities.
        /// If you use retain manually you also have to
        /// release it manually at some point.
        public void Release(object owner)
        {
            _aerc.Release(owner);

            // TODO VD PERFORMANCE
            // _toStringCache = null;

            if (_aerc.RetainCount == 0)
                OnEntityReleased?.Invoke(this);
        }

        // Dispatches OnDestroyEntity which will start the destroy process.
        public void Destroy()
        {
            if (!_isEnabled)
                throw new EntityIsNotEnabledException($"Cannot destroy {this}!");

            OnDestroyEntity?.Invoke(this);
        }

        // This method is used internally. Don't call it yourself.
        // Use entity.Destroy();
        public void InternalDestroy()
        {
            _isEnabled = false;
            RemoveAllComponents();
            OnComponentAdded = null;
            OnComponentReplaced = null;
            OnComponentRemoved = null;
            OnDestroyEntity = null;
        }

        // Do not call this method manually. This method is called by the context.
        public void RemoveAllOnEntityReleasedHandlers() => OnEntityReleased = null;

        /// Returns a cached string to describe the entity
        /// with the following format:
        /// Entity_{creationIndex}(*{retainCount})({list of components})
        public override string ToString()
        {
            if (_toStringCache == null)
            {
                _toStringBuilder ??= new StringBuilder();
                _toStringBuilder.Length = 0;
                _toStringBuilder
                    .Append("Entity_")
                    .Append(_creationIndex)

                    // TODO VD PERFORMANCE
//                    .Append("(*")
//                    .Append(retainCount)
//                    .Append(")")
                    .Append("(");

                const string separator = ", ";
                var components = GetComponents();
                var lastSeparator = components.Length - 1;
                for (var i = 0; i < components.Length; i++)
                {
                    var component = components[i];
                    // var type = component.GetType();

                    // TODO VD PERFORMANCE
                    _toStringCache = null;

//                    var implementsToString = type.GetMethod("ToString")
//                        .DeclaringType.ImplementsInterface<IComponent>();
//                    _toStringBuilder.Append(
//                        implementsToString
//                            ? component.ToString()
//                            : type.ToCompilableString().RemoveComponentSuffix()
//                    );

                    _toStringBuilder.Append(component.ToString());

                    if (i < lastSeparator)
                        _toStringBuilder.Append(separator);
                }

                _toStringBuilder.Append(")");
                _toStringCache = _toStringBuilder.ToString();
            }

            return _toStringCache;
        }
    }
}
