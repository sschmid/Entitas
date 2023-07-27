namespace Entitas
{
    public interface ICompoundMatcher<TEntity> : IMatcher<TEntity> where TEntity : Entity
    {
        int[] AllOfIndexes { get; }
        int[] AnyOfIndexes { get; }
        int[] NoneOfIndexes { get; }
    }
}
