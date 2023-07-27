using DesperateDevs.Reflection;

namespace Entitas
{
    public static class PublicMemberInfoEntityExtension
    {
        /// Adds copies of all specified components to the target entity.
        /// If replaceExisting is true it will replace exising components.
        public static void CopyTo(this Entity entity, Entity target, bool replaceExisting = false, params int[] indexes)
        {
            var componentIndexes = indexes.Length == 0
                ? entity.GetComponentIndexes()
                : indexes;

            for (var i = 0; i < componentIndexes.Length; i++)
            {
                var index = componentIndexes[i];
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
