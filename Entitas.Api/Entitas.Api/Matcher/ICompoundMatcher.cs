namespace Entitas.Api {

    public interface ICompoundMatcher<TEntity> : IMatcher<TEntity> where TEntity : class, IEntity, new() {

        int[] allOfIndices { get; }
        int[] anyOfIndices { get; }
        int[] noneOfIndices { get; }
    }
}
