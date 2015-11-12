using Entitas;

namespace Entitas {
    public partial class Entity {
        static readonly TestComponent testComponent = new TestComponent();

        public bool isTest {
            get { return HasComponent(TestComponentIds.Test); }
            set {
                if (value != isTest) {
                    if (value) {
                        AddComponent(TestComponentIds.Test, testComponent);
                    } else {
                        RemoveComponent(TestComponentIds.Test);
                    }
                }
            }
        }

        public Entity IsTest(bool value) {
            isTest = value;
            return this;
        }
    }
}

    public partial class TestMatcher {
        static IMatcher _matcherTest;

        public static IMatcher Test {
            get {
                if (_matcherTest == null) {
                    var matcher = (Matcher)Matcher.AllOf(TestComponentIds.Test);
                    matcher.componentNames = TestComponentIds.componentNames;
                    _matcherTest = matcher;
                }

                return _matcherTest;
            }
        }
    }
