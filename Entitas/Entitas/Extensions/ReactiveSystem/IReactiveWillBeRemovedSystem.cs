namespace Entitas {
    public interface IReactiveWillBeRemovedSystem {
        AllOfMatcher GetTriggeringMatcher();

        void Execute(EntityComponentPair[] pairs);
    }
}

