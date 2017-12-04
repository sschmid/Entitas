using DesperateDevs.Utils;

namespace Entitas {

    public static class PublicMemberInfoEntityExtension {

        /// Adds copies of all specified components to the target entity.
        /// If replaceExisting is true it will replace exisintg components.
        public static void CopyTo(this IEntity entity, IEntity target, bool replaceExisting = false, params int[] indices) {
            var componentIndices = indices.Length == 0
                ? entity.GetComponentIndices()
                : indices;
            for (int i = 0; i < componentIndices.Length; i++) {
                var index = componentIndices[i];
                var component = entity.GetComponent(index);
                var clonedComponent = target.CreateComponent(index, component.GetType());
                component.CopyPublicMemberValues(clonedComponent);

                if (replaceExisting) {
                    target.ReplaceComponent(index, clonedComponent);
                } else {
                    target.AddComponent(index, clonedComponent);
                }
            }
        }
    }
}
