namespace Entitas.Api {

    public interface IMatcher<TEntity> where TEntity : class, IEntity, new() {

        int[] indices { get; }
        bool Matches(TEntity entity);
    }
}
