public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherDontGenerateMethods;

    public static Entitas.IMatcher<TestEntity> DontGenerateMethods {
        get {
            if(_matcherDontGenerateMethods == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.DontGenerateMethods);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherDontGenerateMethods = matcher;
            }

            return _matcherDontGenerateMethods;
        }
    }
}
