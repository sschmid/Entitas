namespace Entitas {

    public class ContextDoesNotContainEntityException : EntitasException {

        public ContextDoesNotContainEntityException(string message, string hint)
            : base(message + "\nContext does not contain entity!", hint) {
        }
    }
}
