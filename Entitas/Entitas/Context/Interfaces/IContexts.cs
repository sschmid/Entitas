namespace Entitas {

    public interface IContexts {

        IContext[] allContexts { get; }

        void SetAllContexts();
    }
}
