using System.Collections;
using Entitas;
using Entitas.Unity.Serialization.Blueprints;
using UnityEngine;

public class BlueprintsExampleController : MonoBehaviour {

    public Blueprints blueprints;

    void Start() {

        Pools.sharedInstance.blueprints = Pools.CreateBlueprintsPool();

        var max = Pools.sharedInstance.blueprints
            .CreateEntity()
            .ApplyBlueprint(blueprints.Max);

        Debug.Log("max: " + max);

        var jack = Pools.sharedInstance.blueprints
            .CreateEntity()
            .ApplyBlueprint(blueprints.Jack);

        Debug.Log("jack: " + jack);

        StartCoroutine(createMax());
    }

    IEnumerator createMax() {
        while (true) {

            var max = Pools.sharedInstance.blueprints
                .CreateEntity()
                .ApplyBlueprint(blueprints.Max);

            Debug.Log(max.name.value + " is " + max.age.value);

            yield return new WaitForSeconds(1f);
        }
    }
}
