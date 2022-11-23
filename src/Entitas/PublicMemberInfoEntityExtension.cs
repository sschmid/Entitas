using DesperateDevs.Reflection;

namespace Entitas
{
    public static class PublicMemberInfoEntityExtension
    {
        /// Adds copies of all specified components to the target entity.
        /// If replaceExisting is true it will replace exising components.
        public static void CopyTo(this IEntity entity, IEntity target, bool replaceExisting = false, params int[] indices)
        {
            var componentIndices = indices.Length == 0
                ? entity.GetComponentIndices()
                : indices;

            foreach (var index in componentIndices)
            {
                var component = entity.GetComponent(index);
                var clonedComponent = target.CreateComponent(index, component.GetType());
                component.CopyPublicMemberValues(clonedComponent);

                if (replaceExisting)
                    target.ReplaceComponent(index, clonedComponent);
                else
                    target.AddComponent(index, clonedComponent);
            }
        }
    }
}
