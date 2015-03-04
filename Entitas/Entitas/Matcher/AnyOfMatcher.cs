namespace Entitas {
    public class AnyOfMatcher : AbstractMatcher {
        public AnyOfMatcher(int[] indices) : base(indices) {
        }

        public override bool Matches(Entity entity) {
            return entity.HasAnyComponent(indices);
        }
    }
}

