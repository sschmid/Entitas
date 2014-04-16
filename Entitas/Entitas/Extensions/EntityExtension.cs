namespace Entitas {
    public static class EntityExtension {
        public static void HasComponents(this Entity entity, params int[] indices) {
            entity.HasComponents(indices);
        }

        public static void HasAnyComponent(this Entity entity, params int[] indices) {
            entity.HasAnyComponent(indices);
        }
    }
}

