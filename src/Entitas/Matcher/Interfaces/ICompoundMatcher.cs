namespace Entitas
{
    public interface ICompoundMatcher<TEntity> : IMatcher<TEntity> where TEntity : class, IEntity
    {
        int[] AllOfIndices { get; }
        int[] AnyOfIndices { get; }
        int[] NoneOfIndices { get; }
    }
}
