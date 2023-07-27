namespace Entitas
{
    public interface IAnyOfMatcher<TEntity> : INoneOfMatcher<TEntity> where TEntity : Entity
    {
        INoneOfMatcher<TEntity> NoneOf(params int[] indexes);
        INoneOfMatcher<TEntity> NoneOf(params IMatcher<TEntity>[] matchers);
    }
}
