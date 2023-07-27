namespace Entitas
{
    public class EntityIsNotRetainedByOwnerException : EntitasException
    {
        public EntityIsNotRetainedByOwnerException(Entity entity, object owner) :
            base($"'{owner}' cannot release {entity}!\nEntity is not retained by this object!",
                "An entity can only be released from objects that retain it.") { }
    }
}
