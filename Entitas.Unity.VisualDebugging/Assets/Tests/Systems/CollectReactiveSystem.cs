using Entitas;

public class CollectReactiveSystem : IReactiveSystem {
    public IMatcher trigger { get { return Matcher.MyString; } }

    public GroupEventType eventType { get { return GroupEventType.OnEntityAdded; } }

    public void Execute(Entity[] entities) {
        UnityEngine.Debug.Log("entities.Length: " + entities.Length);
    }
}

