namespace Entitas {

    public class ContextEntityIndexDoesNotExistException : EntitasException {

        public ContextEntityIndexDoesNotExistException(IContext context, string name)
            : base("Cannot get EntityIndex '" + name + "' from context '" +
                   context + "'!", "No EntityIndex with this name has been added.") {
        }
    }
}
