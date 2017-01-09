using System.Collections;
using Entitas;
using Entitas.Unity.Serialization.Blueprints;
using UnityEngine;

public class BlueprintsExampleController : MonoBehaviour {

    public Blueprints blueprints;

    void Start() {

        Contexts.sharedInstance.blueprints = Contexts.CreateBlueprintsContext();

        var max = Contexts.sharedInstance.blueprints
            .CreateEntity()
            .ApplyBlueprint(blueprints.Max);

        Debug.Log("max: " + max);

        var jack = Contexts.sharedInstance.blueprints
            .CreateEntity()
            .ApplyBlueprint(blueprints.Jack);

        Debug.Log("jack: " + jack);

        StartCoroutine(createMax());
    }

    IEnumerator createMax() {
        while (true) {

            var max = Contexts.sharedInstance.blueprints
                .CreateEntity()
                .ApplyBlueprint(blueprints.Max);

            Debug.Log(max.name.value + " is " + max.age.value);

            yield return new WaitForSeconds(1f);
        }
    }
}
