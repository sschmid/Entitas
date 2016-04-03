using Entitas.Serialization.Blueprints;

namespace Entitas {

    public partial class Entity {

        public Entity ApplyBlueprint(Blueprint blueprint) {
            for (int i = 0, componentsLength = blueprint.components.Length; i < componentsLength; i++) {
                var componentBlueprint = blueprint.components[i];
                AddComponent(componentBlueprint.index, componentBlueprint.CreateComponent(this));
            }

            return this;
        }
    }
}

