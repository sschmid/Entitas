using Entitas.Serialization.Blueprints;
using Entitas.Unity.Serialization.Blueprints;
using UnityEngine;

public class BlueprintsExampleController : MonoBehaviour {

    public BinaryBlueprint binaryBlueprint;

    Blueprint _blueprint;

    void Start() {
        _blueprint = binaryBlueprint.Deserialize();
    }

    void Update() {
        var pool = Pools.pool;
        for (int i = 0; i < 1; i++) {
            var entity = pool.CreateEntity();
            entity.ApplyBlueprint(_blueprint);
        }
    }
}
