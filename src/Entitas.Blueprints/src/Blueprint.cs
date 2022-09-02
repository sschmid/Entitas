using System;

namespace Entitas.Blueprints {

    [Serializable]
    public class Blueprint {

        public string contextIdentifier;
        public string name;
        public ComponentBlueprint[] components;

        public Blueprint() {
        }

        public Blueprint(string contextIdentifier, string name, IEntity entity) {
            this.contextIdentifier = contextIdentifier;
            this.name = name;

            if (entity != null) {
                var allComponents = entity.GetComponents();
                var componentIndices = entity.GetComponentIndices();
                components = new ComponentBlueprint[allComponents.Length];
                for (int i = 0; i < allComponents.Length; i++) {
                    components[i] = new ComponentBlueprint(
                        componentIndices[i], allComponents[i]
                    );
                }
            } else {
                components = new ComponentBlueprint[0];
            }
        }
    }
}
