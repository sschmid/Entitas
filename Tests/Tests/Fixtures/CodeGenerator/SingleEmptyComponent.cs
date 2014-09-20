using Entitas.CodeGenerator;
using Entitas;

[SingleEntity]
public class SingleEmptyComponent : IComponent {
    public static string extensions =
        @"using Entitas;

public static class SingleEmptyComponentGeneratedExtension {

    public static SingleEmptyComponent instance = new SingleEmptyComponent();

    public static void FlagAsSingleEmpty(this Entity entity) {
        entity.AddComponent(ComponentIds.SingleEmpty, instance);
    }

    public static Entity AddSingleSingleEmpty(this EntityRepository repo) {
        if (repo.GetSingleEntity(ComponentIds.SingleEmpty) != null) {
            throw new SingleEntityException(EntityMatcher.AllOf(new [] { ComponentIds.SingleEmpty }));
        }

        var entity = repo.GetEntityFromPool();
        entity.AddComponent(ComponentIds.SingleEmpty, instance);
        return entity;
    }

    public static bool IsSingleEmpty(this Entity entity) {
        return entity.HasComponent(ComponentIds.SingleEmpty);
    }

    public static bool HasSingleSingleEmpty(this EntityRepository repo) {
        return repo.GetSingleEntity(ComponentIds.SingleEmpty) != null;
    }

    public static void UnflagSingleEmpty(this Entity entity) {
        entity.RemoveComponent(ComponentIds.SingleEmpty);
    }

    public static void RemoveSingleSingleEmpty(this EntityRepository repo) {
        var entity = repo.GetSingleEntity(ComponentIds.SingleEmpty);
        repo.PushToPool(entity);
    }

    public static Entity GetSingleSingleEmptyEntity(this EntityRepository repo) {
        return repo.GetSingleEntity(ComponentIds.SingleEmpty);
    }

}";
}
