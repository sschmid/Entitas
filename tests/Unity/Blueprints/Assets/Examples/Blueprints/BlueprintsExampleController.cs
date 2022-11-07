using System.Collections;
using Entitas.Blueprints;
using Entitas.Blueprints.Unity;
using UnityEngine;

public class BlueprintsExampleController : MonoBehaviour {

    public Blueprints blueprints;

    void Start() {

        var context = Contexts.sharedInstance.game;

        var max = context.CreateEntity();
        max.ApplyBlueprint(blueprints.Max());
        Debug.Log("max: " + max);

        var jack = context.CreateEntity();
        jack.ApplyBlueprint(blueprints.Jack());
        Debug.Log("jack: " + jack);

        StartCoroutine(createMax());
    }

    IEnumerator createMax() {
        while (true) {
            var max = Contexts.sharedInstance.game.CreateEntity();
            max.ApplyBlueprint(blueprints.Max());

            Debug.Log(max.name.value + " is " + max.age.value);

            yield return new WaitForSeconds(1f);
        }
    }
}
