using Entitas.Serialization.Blueprints;

namespace Entitas {

    public partial class Entity {

        /// Adds all components from the blueprint to the entity.
        /// When 'replaceComponents' is set to true entity.ReplaceComponent() will be used instead of entity.AddComponent().
        public Entity ApplyBlueprint(Blueprint blueprint, bool replaceComponents = false) {
            for (int i = 0, componentsLength = blueprint.components.Length; i < componentsLength; i++) {
                var componentBlueprint = blueprint.components[i];
                if (replaceComponents) {
                    ReplaceComponent(componentBlueprint.index, componentBlueprint.CreateComponent(this));
                } else {
                    AddComponent(componentBlueprint.index, componentBlueprint.CreateComponent(this));
                }
            }

            return this;
        }
    }
}

