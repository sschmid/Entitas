using Entitas.Blueprints;
using Entitas.Unity.Blueprints;

public static class BlueprintsExtension {

    public static Blueprint Jack(this Blueprints blueprints) {
        return blueprints.GetBlueprint("Jack");
    }
    public static Blueprint Max(this Blueprints blueprints) {
        return blueprints.GetBlueprint("Max");
    }
}
