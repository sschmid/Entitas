namespace Entitas {

    public class EntityIndexException : EntitasException {

        public EntityIndexException(string message, string hint)
            : base(message, hint) {
        }
    }
}
