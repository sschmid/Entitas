using Entitas;

public class MovableComponent : IComponent {
    public static string extensions =
        @"using Entitas;

public static class MovableComponentGeneratedExtension {

    public static MovableComponent instance = new MovableComponent();

    public static void FlagMovable(this Entity entity) {
        entity.AddComponent(ComponentIds.Movable, instance);
    }

    public static bool IsMovable(this Entity entity) {
        return entity.HasComponent(ComponentIds.Movable);
    }

    public static void UnflagMovable(this Entity entity) {
        entity.RemoveComponent(ComponentIds.Movable);
    }

}";
}
