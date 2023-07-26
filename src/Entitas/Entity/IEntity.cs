using System;
using System.Collections.Generic;

namespace Entitas
{
    public delegate void EntityComponentChanged(
        IEntity entity, int index, IComponent component
    );

    public delegate void EntityComponentReplaced(
        IEntity entity, int index, IComponent previousComponent, IComponent newComponent
    );

    public delegate void EntityEvent(IEntity entity);

    public interface IEntity : IAERC
    {
        event EntityComponentChanged OnComponentAdded;
        event EntityComponentChanged OnComponentRemoved;
        event EntityComponentReplaced OnComponentReplaced;
        event EntityEvent OnEntityReleased;
        event EntityEvent OnDestroyEntity;

        int TotalComponents { get; }
        int Id { get; }
        bool IsEnabled { get; }

        Stack<IComponent>[] ComponentPools { get; }
        ContextInfo ContextInfo { get; }
        IAERC Aerc { get; }

        void Initialize(int creationIndex,
            int totalComponents,
            Stack<IComponent>[] componentPools,
            ContextInfo contextInfo = null,
            IAERC aerc = null);

        void Reuse(int creationIndex);

        void AddComponent(int index, IComponent component);
        void RemoveComponent(int index);
        void ReplaceComponent(int index, IComponent component);

        IComponent GetComponent(int index);
        IComponent[] GetComponents();
        int[] GetComponentIndexes();

        bool HasComponent(int index);
        bool HasComponents(int[] indexes);
        bool HasAnyComponent(int[] indexes);

        void RemoveAllComponents();

        Stack<IComponent> GetComponentPool(int index);
        IComponent CreateComponent(int index, Type type);
        T CreateComponent<T>(int index) where T : new();

        void Destroy();
        void InternalDestroy();
        void RemoveAllOnEntityReleasedHandlers();
    }
}
