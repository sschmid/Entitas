using System;

namespace Entitas.Serialization.Blueprints {

    [Serializable]
    public struct Blueprint {
        public string name;
        public ComponentBlueprint[] components;

        public Blueprint(string name, Entity entity) {
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