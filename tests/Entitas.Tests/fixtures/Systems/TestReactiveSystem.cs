using System.Collections.Generic;
using Entitas;

public class TestReactiveSystem : ReactiveSystem<Test1Entity> {

    public TestReactiveSystem(IContext<Test1Entity> context) : base(context) { }

    protected override ICollector<Test1Entity> GetTrigger(IContext<Test1Entity> context) {
        return context.CreateCollector(Matcher<Test1Entity>.AllOf(0));
    }

    protected override bool Filter(Test1Entity entity) {
        return true;
    }

    protected override void Execute(List<Test1Entity> entities) {
    }
}
