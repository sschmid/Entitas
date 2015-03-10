using Entitas;

public class MultiDimArrayComponent : IComponent {
    public int[,,] grid;
    public static string extensions =
        @"namespace Entitas {
    public partial class Entity {
        public MultiDimArrayComponent multiDimArray { get { return (MultiDimArrayComponent)GetComponent(ComponentIds.MultiDimArray); } }

        public bool hasMultiDimArray { get { return HasComponent(ComponentIds.MultiDimArray); } }

        public void AddMultiDimArray(MultiDimArrayComponent component) {
            AddComponent(ComponentIds.MultiDimArray, component);
        }

        public void AddMultiDimArray(System.Int32[,,] newGrid) {
            var component = new MultiDimArrayComponent();
            component.grid = newGrid;
            AddMultiDimArray(component);
        }

        public void ReplaceMultiDimArray(System.Int32[,,] newGrid) {
            MultiDimArrayComponent component;
            if (hasMultiDimArray) {
                WillRemoveComponent(ComponentIds.MultiDimArray);
                component = multiDimArray;
            } else {
                component = new MultiDimArrayComponent();
            }
            component.grid = newGrid;
            ReplaceComponent(ComponentIds.MultiDimArray, component);
        }

        public void RemoveMultiDimArray() {
            RemoveComponent(ComponentIds.MultiDimArray);
        }
    }

    public static partial class Matcher {
        static AllOfMatcher _matcherMultiDimArray;

        public static AllOfMatcher MultiDimArray {
            get {
                if (_matcherMultiDimArray == null) {
                    _matcherMultiDimArray = Matcher.AllOf(new [] { ComponentIds.MultiDimArray });
                }

                return _matcherMultiDimArray;
            }
        }
    }
}";
}

