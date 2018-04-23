using System.Collections.Generic;
using Entitas;

public interface MyStringEntity : IEntity, IMyStringEntity, ITestEntity { }

public partial class GameEntity : MyStringEntity {}
public partial class InputEntity : MyStringEntity {}

public class SomeMultiReactiveSystem : MultiReactiveSystem<MyStringEntity, Contexts> {

    public SomeMultiReactiveSystem(Contexts contexts) : base(contexts) { }

    protected override ICollector[] GetTrigger(Contexts contexts) {
        return new ICollector[] {
            contexts.game.CreateCollector(GameMatcher.MyString),
            contexts.input.CreateCollector(InputMatcher.MyString)
        };
    }

    protected override bool Filter(MyStringEntity entity) {
        return true;
    }

    protected override void Execute(List<MyStringEntity> entities) {
        foreach(var e in entities) {

            // Methods are available
            var str = e.myString;
            var isTest = e.isTest;

            UnityEngine.Debug.Log("Processed: " + (e));
        }
    }
}
