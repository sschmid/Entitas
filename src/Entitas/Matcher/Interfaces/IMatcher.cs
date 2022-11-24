namespace Entitas
{
    public interface IMatcher<TEntity> where TEntity : class, IEntity
    {
        int[] Indexes { get; }
        bool Matches(TEntity entity);
    }
}
