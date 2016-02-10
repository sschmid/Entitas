namespace Entitas {
    public partial class Pool {
        public Entity CreateEntity(Blueprint blueprint) {
            var entity = CreateEntity();
            foreach (var component in blueprint.components) {
                entity.AddComponent(component.index, component.CreateComponent());
            }

            return entity;
        }
    }
}