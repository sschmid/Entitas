using UnityEngine;
using Entitas;

public class MultiplePoolsController : MonoBehaviour {

    Pool _poolA;
    Pool _poolB;

	void Start () {
	
        _poolA = new Pool(ComponentIds.TotalComponents, 0, new PoolMetaData("Pool A", ComponentIds.componentNames, ComponentIds.componentTypes));
        new Entitas.Unity.VisualDebugging.PoolObserver(_poolA);

        _poolB = new Pool(ComponentIds.TotalComponents, 0, new PoolMetaData("Pool B", ComponentIds.componentNames, ComponentIds.componentTypes));
        new Entitas.Unity.VisualDebugging.PoolObserver(_poolB);


        _poolA.OnEntityCreated += (pool, entity) => entity.AddMyInt(42);
	}
}
