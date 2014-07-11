﻿using System;
using System.Collections.Generic;

namespace Entitas {
    public class Entity {
		public event EntityChange OnComponentAdded;
		public event EntityChange OnComponentWillBeRemoved;
        public event EntityChange OnComponentRemoved;

        public delegate void EntityChange(Entity entity, int index, IComponent component);

        public ulong creationIndex { get { return _creationIndex; } }

        readonly IComponent[] _components;
        readonly ulong _creationIndex;
        IComponent[] _componentsCache;
        int[] _componentIndicesCache;

        public Entity(int totalComponents) : this(totalComponents, 0) {
        }

        public Entity(int totalComponents, ulong creationIndex) {
            _creationIndex = creationIndex;
            _components = new IComponent[totalComponents];
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

		void replaceComponent(int index, IComponent replacement)
		{			
			var component = _components[index];
			
			if(OnComponentWillBeRemoved != null)
				OnComponentWillBeRemoved(this, index, component);
			if(component != replacement){
				_components[index] = replacement;
				_componentsCache = null;
			}
			if(replacement == null)
				_componentIndicesCache = null;
			if (OnComponentRemoved != null)
				OnComponentRemoved(this, index, component);
		}

        public void RemoveComponent(int index) {
            #if (DEBUG)
            if (!HasComponent(index))
                throw new EntityDoesNotHaveComponentException(string.Format("Cannot remove component at index {0} from {1}.", index, this), index);
            #endif
			
			replaceComponent(index, null);
        }

        public void ReplaceComponent(int index, IComponent component) {
            if (HasComponent(index)) {
				replaceComponent(index, component);
            } 
			AddComponent(index, component);
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
            for (int i = 0, indicesLength = indices.Length; i < indicesLength; i++)
                if (_components[indices[i]] == null)
                    return false;

            return true;
        }

        public bool HasAnyComponent(int[] indices) {
            for (int i = 0, indicesLength = indices.Length; i < indicesLength; i++)
                if (_components[indices[i]] != null)
                    return true;

            return false;
        }

        public IComponent[] GetComponents() {
            if (_componentsCache == null) {
                var components = new List<IComponent>();
                for (int i = 0, componentsLength = _components.Length; i < componentsLength; i++) {
                    var component = _components[i];
                    if (component != null)
                        components.Add(component);
                }

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
            for (int i = 0, indicesLength = indices.Length; i < indicesLength; i++)
                RemoveComponent(indices[i]);
        }

        public override string ToString() {
            const string seperator = ", ";
            var componentsStr = string.Empty;
            var components = GetComponents();
            for (int i = 0, componentsLength = components.Length; i < componentsLength; i++)
                componentsStr += components[i] + seperator;

            if (componentsStr != string.Empty)
                componentsStr = componentsStr.Substring(0, componentsStr.Length - seperator.Length);

            return string.Format("Entity({0})", componentsStr);
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

