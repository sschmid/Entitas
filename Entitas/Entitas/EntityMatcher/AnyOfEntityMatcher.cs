namespace Entitas {
    public class AnyOfEntityMatcher : AbstractEntityMatcher {
		public AnyOfEntityMatcher(int[] indices) : base(indices) {
        }

        public override bool Matches(Entity entity) {
            return entity.HasAnyComponent(indices);
        }
    }
}
