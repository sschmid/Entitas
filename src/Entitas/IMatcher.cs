namespace Entitas
{
    public interface IMatcher<TEntity> where TEntity : class, IEntity
    {
        int[] indexes { get; }
        bool Matches(TEntity entity);
    }
}
