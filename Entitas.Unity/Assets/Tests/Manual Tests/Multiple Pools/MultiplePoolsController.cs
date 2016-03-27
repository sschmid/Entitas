using UnityEngine;
using Entitas;

public class MultiplePoolsController : MonoBehaviour {

    Pool _poolA;
    Pool _poolB;

	void Start () {
	
        _poolA = new Pool(VisualDebuggingComponentIds.TotalComponents, 0, new PoolMetaData("Pool A", VisualDebuggingComponentIds.componentNames, VisualDebuggingComponentIds.componentTypes));
        new Entitas.Unity.VisualDebugging.PoolObserver(_poolA);

        _poolB = new Pool(VisualDebuggingComponentIds.TotalComponents, 0, new PoolMetaData("Pool B", VisualDebuggingComponentIds.componentNames, VisualDebuggingComponentIds.componentTypes));
        new Entitas.Unity.VisualDebugging.PoolObserver(_poolB);


        _poolA.OnEntityCreated += (pool, entity) => entity.AddMyInt(42);
	}
}
