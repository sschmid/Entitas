namespace Entitas {

    public delegate bool MatcherFilter(Entity entity);

    public interface IMatcher {
        int[] indices { get; }
        IMatcher Where(MatcherFilter filter);
        bool Matches(Entity entity);
    }
}
