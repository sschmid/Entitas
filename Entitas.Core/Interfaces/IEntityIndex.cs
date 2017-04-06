namespace Entitas.Core {

    public interface IEntityIndex {

        string name { get; }

        void Activate();
        void Deactivate();
    }
}
