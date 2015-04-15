namespace Entitas {
    public partial class Entity {
        public HealthComponent health { get { return (HealthComponent)GetComponent(ComponentIds.Health); } }

        public bool hasHealth { get { return HasComponent(ComponentIds.Health); } }

        public void AddHealth(HealthComponent component) {
            AddComponent(ComponentIds.Health, component);
        }

        public void AddHealth(int newHealth) {
            var component = new HealthComponent();
            component.health = newHealth;
            AddHealth(component);
        }

        public void ReplaceHealth(int newHealth) {
            HealthComponent component;
            if (hasHealth) {
                WillRemoveComponent(ComponentIds.Health);
                component = health;
            } else {
                component = new HealthComponent();
            }
            component.health = newHealth;
            ReplaceComponent(ComponentIds.Health, component);
        }

        public void RemoveHealth() {
            RemoveComponent(ComponentIds.Health);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherHealth;

        public static AllOfMatcher Health {
            get {
                if (_matcherHealth == null) {
                    _matcherHealth = new Matcher(ComponentIds.Health);
                }

                return _matcherHealth;
            }
        }
    }
}
