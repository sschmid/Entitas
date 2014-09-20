using Entitas.CodeGenerator;
using Entitas;
using System;

[SingleEntity]
public class SingleArgsComponent : IComponent {
    public DateTime arg1;
    public bool arg2;
    public static string extensions =
        @"using Entitas;

public static class SingleArgsComponentGeneratedExtension {

    public static void AddSingleArgs(this Entity entity, System.DateTime arg1, bool arg2) {
        var component = new SingleArgsComponent();
        component.arg1 = arg1;
        component.arg2 = arg2;
        entity.AddComponent(ComponentIds.SingleArgs, component);
    }

    public static Entity AddSingleSingleArgs(this EntityRepository repo, System.DateTime arg1, bool arg2) {
        if (repo.GetSingleEntity(ComponentIds.SingleArgs) != null) {
            throw new SingleEntityException(EntityMatcher.AllOf(new [] { ComponentIds.SingleArgs }));
        }

        var entity = repo.GetEntityFromPool();
        var component = new SingleArgsComponent();
        component.arg1 = arg1;
        component.arg2 = arg2;
        entity.AddComponent(ComponentIds.SingleArgs, component);
        return entity;
    }

    public static void ReplaceSingleArgs(this Entity entity, SingleArgsComponent component) {
        entity.ReplaceComponent(ComponentIds.SingleArgs, component);
    }

    public static void ReplaceSingleArgs(this Entity entity, System.DateTime arg1, bool arg2) {
        const int componentId = ComponentIds.SingleArgs;
        SingleArgsComponent component;
        if (entity.HasComponent(componentId)) {
            component = (SingleArgsComponent)entity.GetComponent(componentId);
        } else {
            component = new SingleArgsComponent();
        }
        component.arg1 = arg1;
        component.arg2 = arg2;
        entity.ReplaceComponent(componentId, component);
    }

    public static Entity ReplaceSingleSingleArgs(this EntityRepository repo, SingleArgsComponent component) {
        const int componentId = ComponentIds.SingleArgs;
        Entity entity = repo.GetSingleEntity(componentId);
        if (entity == null) {
            entity = repo.GetEntityFromPool();
            entity.AddComponent(componentId, component);
        } else {
            entity.ReplaceComponent(componentId, component);
        }
        return entity;
    }

    public static Entity ReplaceSingleSingleArgs(this EntityRepository repo, System.DateTime arg1, bool arg2) {
        const int componentId = ComponentIds.SingleArgs;
        Entity entity = repo.GetSingleEntity(componentId);
        SingleArgsComponent component;
        if (entity == null) {
            entity = repo.GetEntityFromPool();
            component = new SingleArgsComponent();
        } else {
            component = (SingleArgsComponent)entity.GetComponent(componentId);
        }
        component.arg1 = arg1;
        component.arg2 = arg2;
        entity.ReplaceComponent(componentId, component);
        return entity;
    }

    public static bool HasSingleArgs(this Entity entity) {
        return entity.HasComponent(ComponentIds.SingleArgs);
    }

    public static bool HasSingleSingleArgs(this EntityRepository repo) {
        return repo.GetSingleEntity(ComponentIds.SingleArgs) != null;
    }

    public static void RemoveSingleArgs(this Entity entity) {
        entity.RemoveComponent(ComponentIds.SingleArgs);
    }

    public static void RemoveSingleSingleArgs(this EntityRepository repo) {
        var entity = repo.GetSingleEntity(ComponentIds.SingleArgs);
        repo.PushToPool(entity);
    }

    public static SingleArgsComponent GetSingleArgs(this Entity entity) {
        return (SingleArgsComponent)entity.GetComponent(ComponentIds.SingleArgs);
    }

    public static SingleArgsComponent GetSingleSingleArgs(this EntityRepository repo) {
        const int componentId = ComponentIds.SingleArgs;
        var entity = repo.GetSingleEntity(componentId);
        return (SingleArgsComponent)entity.GetComponent(componentId);
    }

    public static Entity GetSingleSingleArgsEntity(this EntityRepository repo) {
        return repo.GetSingleEntity(ComponentIds.SingleArgs);
    }

}";
}
