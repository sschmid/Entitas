namespace Entitas {
    public interface IMultiReactiveSystem : IReactiveExecuteSystem {
        TriggerOnEvent[] triggers { get; }
    }
}

