using System;

namespace Entitas.Serialization.Blueprints {

    [Serializable]
    public class Blueprint {
        public string poolIdentifier;
        public string name;
        public ComponentBlueprint[] components;

        public Blueprint() {
        }

        public Blueprint(string poolIdentifier, string name, Entity entity) {
            this.poolIdentifier = poolIdentifier;
            this.name = name;

            var allComponents = entity.GetComponents();
            var componentIndices = entity.GetComponentIndices();
            components = new ComponentBlueprint[allComponents.Length];
            for (int i = 0, componentsLength = allComponents.Length; i < componentsLength; i++) {
                components[i] = new ComponentBlueprint(componentIndices[i], allComponents[i]);
            }
        }
    }
}
