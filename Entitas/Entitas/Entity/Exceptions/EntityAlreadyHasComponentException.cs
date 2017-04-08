namespace Entitas {

    public class EntityAlreadyHasComponentException : EntitasException {

        public EntityAlreadyHasComponentException(int index, string message, string hint)
            : base(message + "\nEntity already has a component at index " + index + "!", hint) {
        }
    }
}
