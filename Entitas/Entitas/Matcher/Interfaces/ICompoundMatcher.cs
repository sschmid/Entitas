namespace Entitas {

    public interface ICompoundMatcher<TEntity> : IMatcher<TEntity> where TEntity : class, IEntity {

        int[] allOfIndices { get; }
        int[] anyOfIndices { get; }
        int[] noneOfIndices { get; }
    }
}
