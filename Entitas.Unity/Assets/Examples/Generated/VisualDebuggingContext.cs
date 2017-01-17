namespace Entitas {

    public partial class VisualDebuggingContext : XXXContext<VisualDebuggingEntity> {

        public VisualDebuggingContext(int totalComponents, int startCreationIndex, ContextInfo contextInfo)
            : base(totalComponents, startCreationIndex, contextInfo) {
        }
    }
}
