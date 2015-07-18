using Entitas;

public class ProcessRandomValueSystem : IReactiveSystem {
    public IMatcher trigger { get { return Matcher.MyFloat; } }

    public GroupEventType eventType { get { return GroupEventType.OnEntityAdded; } }

    public void Execute(Entity[] entities) {
        UnityEngine.Debug.Log("entities.Length: " + entities.Length);
    }
}

