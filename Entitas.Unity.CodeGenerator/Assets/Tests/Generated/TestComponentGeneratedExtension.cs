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
        static AllOfMatcher _matcherTest;

        public static AllOfMatcher Test {
            get {
                if (_matcherTest == null) {
                    _matcherTest = new TestMatcher(TestComponentIds.Test);
                }

                return _matcherTest;
            }
        }
    }
