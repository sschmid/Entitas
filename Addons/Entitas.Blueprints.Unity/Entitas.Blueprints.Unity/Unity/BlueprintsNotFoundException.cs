namespace Entitas.Blueprints.Unity {

    public class BlueprintsNotFoundException : EntitasException {

        public BlueprintsNotFoundException(string blueprintName)
            : base("'" + blueprintName + "' does not exist!", "Did you update the Blueprints ScriptableObject?") {
        }
    }
}
