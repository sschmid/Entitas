using Entitas.CodeGenerator;
using Entitas;

[SingleEntity]
public class AnimatingComponent : IComponent {
    public static string extensions =
        @"using Entitas;

public static class AnimatingComponentGeneratedExtension {

    public static AnimatingComponent instance = new AnimatingComponent();

    public static void FlagAnimating(this Entity entity) {
        entity.AddComponent(ComponentIds.Animating, instance);
    }

    public static Entity FlagAnimating(this EntityRepository repo) {
        if (repo.GetSingleEntity(ComponentIds.Animating) != null) {
            throw new SingleEntityException(EntityMatcher.AllOf(new [] { ComponentIds.Animating }));
        }

        var entity = repo.CreateEntity();
        entity.AddComponent(ComponentIds.Animating, instance);
        return entity;
    }

    public static bool IsAnimating(this Entity entity) {
        return entity.HasComponent(ComponentIds.Animating);
    }

    public static bool IsAnimating(this EntityRepository repo) {
        return repo.GetSingleEntity(ComponentIds.Animating) != null;
    }

    public static void UnflagAnimating(this Entity entity) {
        entity.RemoveComponent(ComponentIds.Animating);
    }

    public static void UnflagAnimating(this EntityRepository repo) {
        var entity = repo.GetSingleEntity(ComponentIds.Animating);
        repo.DestroyEntity(entity);
    }

    public static Entity GetAnimatingEntity(this EntityRepository repo) {
        return repo.GetSingleEntity(ComponentIds.Animating);
    }

}";
}
