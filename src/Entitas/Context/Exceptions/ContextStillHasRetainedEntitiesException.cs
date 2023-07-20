using System.Collections.Generic;
using System.Linq;

namespace Entitas
{
    public class ContextStillHasRetainedEntitiesException : EntitasException
    {
        public ContextStillHasRetainedEntitiesException(IContext context, IEnumerable<IEntity> entities) : base(
            $"'{context}' detected retained entities although all entities got destroyed!",
            $"Did you release all entities? Try calling systems.DeactivateReactiveSystems() before calling context.DestroyAllEntities() to avoid memory leaks. Do not forget to activate them back when needed.\n{EntitiesToString(entities)}") { }

        static string EntitiesToString(IEnumerable<IEntity> entities) => string.Join("\n",
            entities.Select(e => e.aerc is SafeAERC safeAerc
                ? $"{e} - {string.Join(", ", safeAerc.owners.Select(o => o.ToString()))}"
                : e.ToString())
        );
    }
}
