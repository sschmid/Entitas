namespace Entitas {

    public interface IEntityIndex {

        string name { get; }

        void Activate();
        void Deactivate();
    }
}
