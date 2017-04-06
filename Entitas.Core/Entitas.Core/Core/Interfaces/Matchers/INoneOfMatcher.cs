namespace Entitas.Core {

    public interface INoneOfMatcher<TEntity> : ICompoundMatcher<TEntity> where TEntity : class, IEntity, new() {
    }
}
