using Entitas.Unity.Serialization.Blueprints;
using UnityEngine;

public class BlueprintsExampleController : MonoBehaviour {

    public Blueprints blueprints;

    void Start() {
        var max = Pools.blueprints
            .CreateEntity()
            .ApplyBlueprint(blueprints.GetBlueprint("Max"));

        Debug.Log("max: " + max);

        var jack = Pools.blueprints
            .CreateEntity()
            .ApplyBlueprint(blueprints.GetBlueprint("Jack"));

        Debug.Log("jack: " + jack);
    }
}
