namespace Entitas
{
    public interface IMatcher<TEntity> where TEntity : class, IEntity
    {
        int[] Indices { get; }
        bool Matches(TEntity entity);
    }
}
