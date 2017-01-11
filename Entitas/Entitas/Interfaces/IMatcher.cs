namespace Entitas {

    public interface IMatcher {
        int[] indices { get; }
        bool Matches(IEntity entity);
    }
}
