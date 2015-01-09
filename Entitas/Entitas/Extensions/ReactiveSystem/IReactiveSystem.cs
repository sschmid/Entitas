namespace Entitas {
    public interface IReactiveSystem {
        IMatcher GetTriggeringMatcher();

        GroupEventType GetEventType();

        void Execute(Entity[] entities);
    }
}

