namespace Entitas
{
    public interface IAnyOfMatcher<TEntity> : INoneOfMatcher<TEntity> where TEntity : class, IEntity
    {
        INoneOfMatcher<TEntity> NoneOf(params int[] indexes);
        INoneOfMatcher<TEntity> NoneOf(params IMatcher<TEntity>[] matchers);
    }
}
