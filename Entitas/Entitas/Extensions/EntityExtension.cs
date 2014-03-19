namespace Entitas {
    public static class EntityExtension {
        public static void AddComponent<T>(this Entity entity) where T : IComponent, new() {
            entity.AddComponent(new T());
        }

        public static void RemoveComponent<T>(this Entity entity) {
            entity.RemoveComponent(typeof(T));
        }

        public static void ReplaceComponent<T>(this Entity entity) where T : IComponent, new() {
            entity.ReplaceComponent(new T());
        }

        public static T GetComponent<T>(this Entity entity) where T : IComponent {
            return (T)entity.GetComponent(typeof(T));
        }

        public static bool HasComponent<T>(this Entity entity) {
            return entity.HasComponent(typeof(T));
        }
    }
}
