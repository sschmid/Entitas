namespace Entitas.Blueprints {

    public static class BlueprintEntityExtension {

        /// Adds all components from the blueprint to the entity.
        /// When 'replaceComponents' is set to true entity.ReplaceComponent()
        /// will be used instead of entity.AddComponent().
        public static void ApplyBlueprint(this IEntity entity, Blueprint blueprint,
            bool replaceComponents = false) {
            var componentsLength = blueprint.components.Length;
            for (int i = 0; i < componentsLength; i++) {
                var componentBlueprint = blueprint.components[i];
                if (replaceComponents) {
                    entity.ReplaceComponent(componentBlueprint.index,
                        componentBlueprint.CreateComponent(entity));
                } else {
                    entity.AddComponent(componentBlueprint.index,
                        componentBlueprint.CreateComponent(entity));
                }
            }
        }
    }
}
