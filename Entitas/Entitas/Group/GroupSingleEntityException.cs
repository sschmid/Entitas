using System.Linq;
using Entitas.Core;

namespace Entitas {

    public class GroupSingleEntityException<TEntity> : EntitasException where TEntity : class, IEntity, new() {

        public GroupSingleEntityException(IGroup<TEntity> group)
            : base("Cannot get the single entity from " + group +
                "!\nGroup contains " + group.count + " entities:",
                string.Join("\n", group.GetEntities().Select(e => e.ToString()).ToArray())) {
        }
    }
}
