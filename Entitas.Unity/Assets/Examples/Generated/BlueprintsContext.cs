namespace Entitas {

    public partial class BlueprintsContext : Context<BlueprintsEntity> {

        public BlueprintsContext(int totalComponents, int startCreationIndex, ContextInfo contextInfo)
            : base(totalComponents, startCreationIndex, contextInfo) {
        }
    }
}
