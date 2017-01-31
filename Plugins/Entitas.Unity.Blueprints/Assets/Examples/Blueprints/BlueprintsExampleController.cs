using System.Collections;
using Entitas;
using Entitas.Blueprints;
using Entitas.Unity.Blueprints;
using UnityEngine;

public class BlueprintsExampleController : MonoBehaviour {

    public Blueprints blueprints;

    void Start() {

        //Contexts.sharedInstance.blueprints = Contexts.CreateBlueprintsContext();

        // TODO

        //var max = Contexts.sharedInstance.blueprints.CreateEntity();
        //max.ApplyBlueprint(blueprints.Max);

        //Debug.Log("max: " + max);

        //var jack = Contexts.sharedInstance.blueprints.CreateEntity();
        //jack.ApplyBlueprint(blueprints.Jack);

        //Debug.Log("jack: " + jack);

        StartCoroutine(createMax());
    }

    IEnumerator createMax() {
        while (true) {


            // TODO
            //var max = Contexts.sharedInstance.blueprints.CreateEntity();
            //max.ApplyBlueprint(blueprints.Max);

            //Debug.Log(max.name.value + " is " + max.age.value);

            yield return new WaitForSeconds(1f);
        }
    }
}
