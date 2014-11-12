namespace Entitas {
    public interface IReactiveSubEntityWillBeRemovedSystem {
        AllOfEntityMatcher GetTriggeringMatcher();

        void Execute(EntityComponentPair[] pairs);
    }
}

