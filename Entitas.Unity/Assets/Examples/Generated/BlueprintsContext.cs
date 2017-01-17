namespace Entitas {

    public partial class BlueprintsContext : XXXContext<BlueprintsEntity> {

        public BlueprintsContext(int totalComponents, int startCreationIndex, ContextInfo contextInfo)
            : base(totalComponents, startCreationIndex, contextInfo) {
        }
    }
}
