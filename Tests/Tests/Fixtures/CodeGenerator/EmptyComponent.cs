using Entitas;

public class EmptyComponent : IComponent {
    public static string extensions =
        @"using Entitas;

public static class EmptyComponentGeneratedExtension {

    public static EmptyComponent instance = new EmptyComponent();

    public static void FlagAsEmpty(this Entity entity) {
        entity.AddComponent(ComponentIds.Empty, instance);
    }

    public static bool IsEmpty(this Entity entity) {
        return entity.HasComponent(ComponentIds.Empty);
    }

    public static void UnflagEmpty(this Entity entity) {
        entity.RemoveComponent(ComponentIds.Empty);
    }

}";
}
