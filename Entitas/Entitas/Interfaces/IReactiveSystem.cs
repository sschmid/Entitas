namespace Entitas {
    public interface IReactiveSystem : ISystem {
        IMatcher GetTriggeringMatcher();

        GroupEventType GetEventType();

        void Execute(Entity[] entities);
    }
}

