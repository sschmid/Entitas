namespace Entitas {
    public class AllOfEntityMatcher : AbstractEntityMatcher {
        public AllOfEntityMatcher(int[] indices) : base(indices) {
        }

        public override bool Matches(Entity entity) {
            return entity.HasComponents(indices);
        }
    }
}
