namespace Entitas
{
    public interface IMatcher<TEntity> where TEntity : Entity
    {
        int[] Indexes { get; }
        bool Matches(TEntity entity);
    }
}
