namespace Entitas
{
    public interface IEntityIndex
    {
        string Name { get; }

        void Activate();
        void Deactivate();
    }
}
