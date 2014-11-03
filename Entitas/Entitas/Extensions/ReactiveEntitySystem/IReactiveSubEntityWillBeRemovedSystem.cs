namespace Entitas {
    public interface IReactiveSubEntityWillBeRemovedSystem {
        int GetTriggeringIndex();

        void Execute(EntityComponentPair[] pairs);
    }
}

