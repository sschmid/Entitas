namespace Entitas {

    public class ContextInfoException : EntitasException {

        public ContextInfoException(IContext context, ContextInfo contextInfo)
            : base("Invalid ContextInfo for '" + context + "'!\nExpected " +
                context.totalComponents + " componentName(s) but got " +
                contextInfo.componentNames.Length + ":",
                string.Join("\n", contextInfo.componentNames)) {
        }
    }
}
