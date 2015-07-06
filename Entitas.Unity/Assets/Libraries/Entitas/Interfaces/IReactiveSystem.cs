namespace Entitas {
    public interface IReactiveSystem : ISystem {
        IMatcher trigger { get; }

        GroupEventType eventType { get; }

        void Execute(Entity[] entities);
    }
}

