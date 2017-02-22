public partial class TestEntity {

    static readonly FlagComponent flagComponent = new FlagComponent();

    public bool isFlag {
        get { return HasComponent(TestComponentsLookup.Flag); }
        set {
            if(value != isFlag) {
                if(value) {
                    AddComponent(TestComponentsLookup.Flag, flagComponent);
                } else {
                    RemoveComponent(TestComponentsLookup.Flag);
                }
            }
        }
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherFlag;

    public static Entitas.IMatcher<TestEntity> Flag {
        get {
            if(_matcherFlag == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.Flag);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherFlag = matcher;
            }

            return _matcherFlag;
        }
    }
}
