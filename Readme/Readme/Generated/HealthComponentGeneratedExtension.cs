namespace Entitas {
    public partial class Entity {
        public HealthComponent health { get { return (HealthComponent)GetComponent(ComponentIds.Health); } }

        public bool hasHealth { get { return HasComponent(ComponentIds.Health); } }

        public Entity AddHealth(HealthComponent component) {
            return AddComponent(ComponentIds.Health, component);
        }

        public Entity AddHealth(int newHealth) {
            var component = new HealthComponent();
            component.health = newHealth;
            return AddHealth(component);
        }

        public Entity ReplaceHealth(int newHealth) {
            HealthComponent component;
            if (hasHealth) {
                component = health;
            } else {
                component = new HealthComponent();
            }
            component.health = newHealth;
            return ReplaceComponent(ComponentIds.Health, component);
        }

        public Entity RemoveHealth() {
            return RemoveComponent(ComponentIds.Health);
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
