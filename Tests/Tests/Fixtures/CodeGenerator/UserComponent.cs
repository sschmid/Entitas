using Entitas.CodeGenerator;
using Entitas;
using System;

[SingleEntity]
public class UserComponent : IComponent {
    public DateTime timestamp;
    public bool isLoggedId;
    public static string extensions =
        @"using Entitas;

public static class UserComponentGeneratedExtension {

    public static void AddUser(this Entity entity, System.DateTime timestamp, bool isLoggedId) {
        var component = new UserComponent();
        component.timestamp = timestamp;
        component.isLoggedId = isLoggedId;
        entity.AddComponent(ComponentIds.User, component);
    }

    public static Entity SetUser(this EntityRepository repo, System.DateTime timestamp, bool isLoggedId) {
        if (repo.GetSingleEntity(ComponentIds.User) != null) {
            throw new SingleEntityException(EntityMatcher.AllOf(new [] { ComponentIds.User }));
        }

        var entity = repo.CreateEntity();
        var component = new UserComponent();
        component.timestamp = timestamp;
        component.isLoggedId = isLoggedId;
        entity.AddComponent(ComponentIds.User, component);
        return entity;
    }

    public static void ReplaceUser(this Entity entity, UserComponent component) {
        entity.ReplaceComponent(ComponentIds.User, component);
    }

    public static void ReplaceUser(this Entity entity, System.DateTime timestamp, bool isLoggedId) {
        const int componentId = ComponentIds.User;
        UserComponent component;
        if (entity.HasComponent(componentId)) {
            entity.WillRemoveComponent(componentId);
            component = (UserComponent)entity.GetComponent(componentId);
        } else {
            component = new UserComponent();
        }
        component.timestamp = timestamp;
        component.isLoggedId = isLoggedId;
        entity.ReplaceComponent(componentId, component);
    }

    public static Entity ReplaceUser(this EntityRepository repo, UserComponent component) {
        const int componentId = ComponentIds.User;
        Entity entity = repo.GetSingleEntity(componentId);
        if (entity == null) {
            entity = repo.CreateEntity();
            entity.AddComponent(componentId, component);
        } else {
            entity.ReplaceComponent(componentId, component);
        }
        return entity;
    }

    public static Entity ReplaceUser(this EntityRepository repo, System.DateTime timestamp, bool isLoggedId) {
        const int componentId = ComponentIds.User;
        Entity entity = repo.GetSingleEntity(componentId);
        UserComponent component;
        if (entity == null) {
            entity = repo.CreateEntity();
            component = new UserComponent();
        } else {
            entity.WillRemoveComponent(componentId);
            component = (UserComponent)entity.GetComponent(componentId);
        }
        component.timestamp = timestamp;
        component.isLoggedId = isLoggedId;
        entity.ReplaceComponent(componentId, component);
        return entity;
    }

    public static bool HasUser(this Entity entity) {
        return entity.HasComponent(ComponentIds.User);
    }

    public static bool HasUser(this EntityRepository repo) {
        return repo.GetSingleEntity(ComponentIds.User) != null;
    }

    public static void RemoveUser(this Entity entity) {
        entity.RemoveComponent(ComponentIds.User);
    }

    public static void RemoveUser(this EntityRepository repo) {
        var entity = repo.GetSingleEntity(ComponentIds.User);
        repo.DestroyEntity(entity);
    }

    public static UserComponent GetUser(this Entity entity) {
        return (UserComponent)entity.GetComponent(ComponentIds.User);
    }

    public static UserComponent GetUser(this EntityRepository repo) {
        const int componentId = ComponentIds.User;
        var entity = repo.GetSingleEntity(componentId);
        return (UserComponent)entity.GetComponent(componentId);
    }

    public static Entity GetUserEntity(this EntityRepository repo) {
        return repo.GetSingleEntity(ComponentIds.User);
    }

}";
}
