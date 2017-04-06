namespace Entitas.Core {

    public interface IAllOfMatcher<TEntity> : IAnyOfMatcher<TEntity> where TEntity : class, IEntity, new() {

        IAnyOfMatcher<TEntity> AnyOf(params int[] indices);
        IAnyOfMatcher<TEntity> AnyOf(params IMatcher<TEntity>[] matchers);
    }
}
