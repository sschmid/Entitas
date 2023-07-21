using System.Linq;

namespace Entitas
{
    public class GroupSingleEntityException<TEntity> : EntitasException where TEntity : class, IEntity
    {
        public GroupSingleEntityException(IGroup<TEntity> group) :
            base($"Cannot get the single entity from {group}!\nGroup contains {group.Count} entities:",
                string.Join("\n", group.GetEntities().Select(entity => entity.ToString()))) { }
    }
}
