namespace Entitas {
    public interface IMultiReactiveSystem : IReactiveExecuteSystem {
        IMatcher[] triggers { get; }
        GroupEventType[] eventTypes { get; }
    }
}

