namespace Entitas
{
    public interface ICompoundMatcher<TEntity> : IMatcher<TEntity> where TEntity : class, IEntity
    {
        int[] allOfIndexes { get; }
        int[] anyOfIndexes { get; }
        int[] noneOfIndexes { get; }
    }
}
