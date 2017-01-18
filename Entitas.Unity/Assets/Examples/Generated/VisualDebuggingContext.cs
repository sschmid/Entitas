namespace Entitas {

    public partial class VisualDebuggingContext : Context<VisualDebuggingEntity> {

        public VisualDebuggingContext(int totalComponents, int startCreationIndex, ContextInfo contextInfo)
            : base(totalComponents, startCreationIndex, contextInfo) {
        }
    }
}
