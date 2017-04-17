namespace Entitas {

    public interface IReactiveSystem : IExecuteSystem {

        void Activate();
        void Deactivate();
        void Clear();
    }
}
