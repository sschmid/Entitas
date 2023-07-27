using System.Collections.Generic;
using System.Linq;

namespace Entitas
{
    public class ContextStillHasRetainedEntitiesException : EntitasException
    {
        public ContextStillHasRetainedEntitiesException(IContext context, IEnumerable<Entity> entities) :
            base($"'{context}' detected retained entities although all entities got destroyed!",
                $"Did you release all entities? Try calling systems.DeactivateReactiveSystems() before calling context.DestroyAllEntities() to avoid memory leaks. Do not forget to activate them back when needed.\n{EntitiesToString(entities)}") { }

        static string EntitiesToString(IEnumerable<Entity> entities) =>
            string.Join("\n", entities.Select(entity => entity.Aerc is SafeAERC safeAerc
                ? $"{entity} - {string.Join(", ", safeAerc.Owners.Select(o => o.ToString()))}"
                : entity.ToString())
            );
    }
}
