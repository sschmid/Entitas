namespace Entitas
{
    public interface IAllOfMatcher<TEntity> : IAnyOfMatcher<TEntity> where TEntity : class, IEntity
    {
        IAnyOfMatcher<TEntity> AnyOf(params int[] indexes);
        IAnyOfMatcher<TEntity> AnyOf(params IMatcher<TEntity>[] matchers);
    }
}
