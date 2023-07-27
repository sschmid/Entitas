namespace Entitas
{
    public class EntityIsAlreadyRetainedByOwnerException : EntitasException
    {
        public EntityIsAlreadyRetainedByOwnerException(Entity entity, object owner) :
            base($"'{owner}' cannot retain {entity}!\nEntity is already retained by this object!",
                "The entity must be released by this object first.") { }
    }
}
