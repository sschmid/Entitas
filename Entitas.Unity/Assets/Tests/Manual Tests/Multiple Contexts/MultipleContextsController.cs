using UnityEngine;
using Entitas;

public class MultipleContextsController : MonoBehaviour {

    Context _contextA;
    Context _contextB;

	void Start () {
	
        _contextA = new Context(VisualDebuggingComponentIds.TotalComponents, 0, new ContextInfo("Context A", VisualDebuggingComponentIds.componentNames, VisualDebuggingComponentIds.componentTypes));
        new Entitas.Unity.VisualDebugging.ContextObserver(_contextA);

        _contextB = new Context(VisualDebuggingComponentIds.TotalComponents, 0, new ContextInfo("Context B", VisualDebuggingComponentIds.componentNames, VisualDebuggingComponentIds.componentTypes));
        new Entitas.Unity.VisualDebugging.ContextObserver(_contextB);


        _contextA.OnEntityCreated += (context, entity) => entity.AddMyInt(42);
	}
}
