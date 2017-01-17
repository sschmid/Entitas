namespace Entitas {

    public interface IMatcher<TEntity> where TEntity : class, IEntity, new() {

        int[] indices { get; }
        bool Matches(TEntity entity);
    }

    public interface ICompoundMatcher<TEntity> : IMatcher<TEntity> where TEntity : class, IEntity, new() {

        int[] allOfIndices { get; }
        int[] anyOfIndices { get; }
        int[] noneOfIndices { get; }
    }

    public interface INoneOfMatcher<TEntity> : ICompoundMatcher<TEntity> where TEntity : class, IEntity, new() {
    }

    public interface IAnyOfMatcher<TEntity> : INoneOfMatcher<TEntity> where TEntity : class, IEntity, new() {

        INoneOfMatcher<TEntity> NoneOf(params int[] indices);
        INoneOfMatcher<TEntity> NoneOf(params IMatcher<TEntity>[] matchers);
    }

    public interface IAllOfMatcher<TEntity> : IAnyOfMatcher<TEntity> where TEntity : class, IEntity, new() {

        IAnyOfMatcher<TEntity> AnyOf(params int[] indices);
        IAnyOfMatcher<TEntity> AnyOf(params IMatcher<TEntity>[] matchers);
    }
}
