using Entitas.Serialization.Blueprints;
using Entitas.Unity.Serialization.Blueprints;
using UnityEngine;

public class BlueprintsExampleController : MonoBehaviour {

    public BinaryBlueprint binaryBlueprint;

    Blueprint _blueprint;

    void Start() {
        _blueprint = binaryBlueprint.Deserialize();
        var entity = Pools.blueprints.CreateEntity();
        entity.ApplyBlueprint(_blueprint);

        Debug.Log("name: " + entity.name.value);
        Debug.Log("age: " + entity.age.value);
    }
}
