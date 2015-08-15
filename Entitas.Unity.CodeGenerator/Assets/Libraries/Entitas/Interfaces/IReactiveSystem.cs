namespace Entitas {
    public interface IReactiveSystem : IReactiveExecuteSystem {
        TriggerOnEvent trigger { get; }
    }
}

