using System;
using System.Collections.Generic;

namespace Entitas {

    public delegate void EntityComponentChanged(
        IEntity entity, int index, IComponent component
    );

    public delegate void EntityComponentReplaced(
        IEntity entity, int index, IComponent previousComponent, IComponent newComponent
    );

    public delegate void EntityEvent(IEntity entity);

    public interface IEntity : IAERC {

        event EntityComponentChanged OnComponentAdded;
        event EntityComponentChanged OnComponentRemoved;
        event EntityComponentReplaced OnComponentReplaced;
        event EntityEvent OnEntityReleased;
        event EntityEvent OnDestroyEntity;

        int totalComponents { get; }
        int creationIndex { get; }
        bool isEnabled { get; }

        Stack<IComponent>[] componentPools { get; }
        ContextInfo contextInfo { get; }
        IAERC aerc { get; }

        void Initialize(int creationIndex,
            int totalComponents,
            Stack<IComponent>[] componentPools,
            ContextInfo contextInfo = null,
            IAERC aerc = null);

        void Reactivate(int creationIndex);

        void AddComponent(int index, IComponent component);
        void RemoveComponent(int index);
        void ReplaceComponent(int index, IComponent component);

        IComponent GetComponent(int index);
        IComponent[] GetComponents();
        int[] GetComponentIndices();

        bool HasComponent(int index);
        bool HasComponents(int[] indices);
        bool HasAnyComponent(int[] indices);

        void RemoveAllComponents();

        Stack<IComponent> GetComponentPool(int index);
        IComponent CreateComponent(int index, Type type);
        T CreateComponent<T>(int index) where T : new();

        void Destroy();
        void InternalDestroy();
        void RemoveAllOnEntityReleasedHandlers();
    }
}
