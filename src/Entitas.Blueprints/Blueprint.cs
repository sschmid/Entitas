using System;

namespace Entitas.Blueprints
{
    [Serializable]
    public class Blueprint
    {
        public string contextIdentifier;
        public string name;
        public ComponentBlueprint[] components;

        public Blueprint() { }

        public Blueprint(string contextIdentifier, string name, IEntity entity)
        {
            this.contextIdentifier = contextIdentifier;
            this.name = name;

            if (entity != null)
            {
                var allComponents = entity.GetComponents();
                var componentIndexes = entity.GetComponentIndexes();
                components = new ComponentBlueprint[allComponents.Length];
                for (var i = 0; i < allComponents.Length; i++)
                {
                    components[i] = new ComponentBlueprint(
                        componentIndexes[i], allComponents[i]
                    );
                }
            }
            else
            {
                components = Array.Empty<ComponentBlueprint>();
            }
        }
    }
}
