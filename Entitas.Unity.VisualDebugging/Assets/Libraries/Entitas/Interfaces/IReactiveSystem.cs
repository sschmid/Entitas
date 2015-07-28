namespace Entitas {
    public interface IReactiveSystem : IReactiveExecuteSystem {
        IMatcher trigger { get; }
        GroupEventType eventType { get; }
    }
}

