namespace Entitas {
    public interface IMatcher {
        int[] indices { get; }
        bool Matches(Entity entity);
    }
}
