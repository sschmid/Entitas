using UnityEngine;
using Entitas;

public class MultipleContextsController : MonoBehaviour {

    IContext _contextA;
    IContext _contextB;

	void Start () {
	
        _contextA = new Context<VisualDebuggingEntity>(VisualDebuggingComponentsLookup.TotalComponents, 0, new ContextInfo("Context A", VisualDebuggingComponentsLookup.componentNames, VisualDebuggingComponentsLookup.componentTypes));
        new Entitas.Unity.VisualDebugging.ContextObserver(_contextA);

        _contextB = new Context<VisualDebuggingEntity>(VisualDebuggingComponentsLookup.TotalComponents, 0, new ContextInfo("Context B", VisualDebuggingComponentsLookup.componentNames, VisualDebuggingComponentsLookup.componentTypes));
        new Entitas.Unity.VisualDebugging.ContextObserver(_contextB);


        _contextA.OnEntityCreated += (context, entity) => ((VisualDebuggingEntity)entity).AddMyInt(42);
	}
}
