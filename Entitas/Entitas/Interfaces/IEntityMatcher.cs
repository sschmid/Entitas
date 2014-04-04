namespace Entitas {
    public interface IEntityMatcher {
        int[] indices { get; }

        bool Matches(Entity entity);
    }
}
