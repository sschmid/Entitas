using UnityEngine;
using Entitas;

public class MultipleContextsController : MonoBehaviour {

    IContext _contextA;
    IContext _contextB;

	void Start () {
	
        _contextA = new XXXContext<VisualDebuggingEntity>(VisualDebuggingComponentIds.TotalComponents, 0, new ContextInfo("Context A", VisualDebuggingComponentIds.componentNames, VisualDebuggingComponentIds.componentTypes));
        new Entitas.Unity.VisualDebugging.ContextObserver(_contextA);

        _contextB = new XXXContext<VisualDebuggingEntity>(VisualDebuggingComponentIds.TotalComponents, 0, new ContextInfo("Context B", VisualDebuggingComponentIds.componentNames, VisualDebuggingComponentIds.componentTypes));
        new Entitas.Unity.VisualDebugging.ContextObserver(_contextB);


        _contextA.OnEntityCreated += (context, entity) => ((VisualDebuggingEntity)entity).AddMyInt(42);
	}
}
