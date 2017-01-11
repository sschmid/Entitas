namespace Entitas {

    public interface IMatcher<TEntity> where TEntity : IEntity {

        int[] indices { get; }
        bool Matches(TEntity entity);
    }

    public interface ICompoundMatcher<TEntity> : IMatcher<TEntity> where TEntity : IEntity {

        int[] allOfIndices { get; }
        int[] anyOfIndices { get; }
        int[] noneOfIndices { get; }
    }

    public interface INoneOfMatcher<TEntity> : ICompoundMatcher<TEntity> where TEntity : IEntity {
    }

    public interface IAnyOfMatcher<TEntity> : INoneOfMatcher<TEntity> where TEntity : IEntity {

        INoneOfMatcher<TEntity> NoneOf(params int[] indices);
        INoneOfMatcher<TEntity> NoneOf(params IMatcher<TEntity>[] matchers);
    }

    public interface IAllOfMatcher<TEntity> : IAnyOfMatcher<TEntity> where TEntity : IEntity {

        IAnyOfMatcher<TEntity> AnyOf(params int[] indices);
        IAnyOfMatcher<TEntity> AnyOf(params IMatcher<TEntity>[] matchers);
    }
}
